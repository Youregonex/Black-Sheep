using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Youregone.SL;
using Youregone.Web;
using Youregone.Utils;
using System.Linq;
using System.Collections;
using System.Threading;
using System;

namespace Youregone.SaveSystem
{
    public class LocalDatabase : MonoBehaviour, IService
    {
        public Action OnLocalDatabaseUpdated;

        private List<ScoreEntry> _personalResults;
        private List<ScoreEntry> _personalAndWebResults;

        public int Highscore { get; private set; }
        public bool InternetConnectionAvailable { get; private set; }
        public List<ScoreEntry> PersonalResults => _personalResults.OrderByDescending(entry => entry.score).ToList();
        public List<ScoreEntry> PersonalAndWebResults => _personalAndWebResults.OrderByDescending(entry => entry.score).ToList();

        private Coroutine _currentCoroutine;
        private CancellationTokenSource _cts;

        public bool DatabaseUpdated { get; private set; }

        private void Awake()
        {
            _personalResults = JsonSaverLoader.LoadScoreHolders();
            _personalAndWebResults = new();
            _cts = new();
            UpdateLocalDatabaseAsync(_cts.Token);
        }

        private void OnDestroy()
        {
            if (_cts != null && !_cts.IsCancellationRequested)
                _cts.Cancel();

            if (_currentCoroutine != null)
                StopCoroutine(_currentCoroutine);
        }

        public void SaveNewScoreEntry(string shortName, int score)
        {
            string fullName = StringEncryptor.GetFullName(shortName);
            ScoreEntry scoreEntry = _personalResults.Find(entry => entry.name == fullName);

            if (scoreEntry != null)
            {
                Debug.Log($"Found entry with same name {scoreEntry.name}");
                if (scoreEntry.score < score)
                {
                    scoreEntry.score = score;
                    Debug.Log($"{scoreEntry.name} changing score");
                }
            }
            else
            {
                scoreEntry = new(fullName, score);
                _personalResults.Add(scoreEntry);
            }

            JsonSaverLoader.SaveScoreHoldersJson(_personalResults);
        }

        public int GetHighscore(bool isPersonal)
        {
            if (!DatabaseUpdated)
                return -1;

            if (isPersonal && _personalResults.Count > 0)
                return _personalResults.OrderByDescending(entry => entry.score).FirstOrDefault().score;
            else if(_personalResults.Count == 0)
                return 0;

            if (_personalAndWebResults.Count > 0)
                return _personalAndWebResults.OrderByDescending(entry => entry.score).FirstOrDefault().score;
            else
                return 0;
        }

        private async void UpdateLocalDatabaseAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            if(!await InternetChecker.IsInternetAvailableAsync())
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

                Debug.Log("No internet connection! Loading from json");
                InternetConnectionAvailable = false;
                _personalResults = JsonSaverLoader.LoadScoreHolders();

                if (_personalResults.Count > 0)
                    Highscore = _personalResults.OrderByDescending(entry => entry.score).FirstOrDefault().score;
                else
                    Highscore = 0;

                _personalAndWebResults = null;
                DatabaseUpdated = true;
                OnLocalDatabaseUpdated?.Invoke();
                return;
            }

            Debug.Log("Internet connection available! Loading from web");
            InternetConnectionAvailable = true;

            Task<List<ScoreEntry>> downloadTask = ScoreWebUploader.DownloadScoreHoldersAsync();

            await downloadTask;

            if (cancellationToken.IsCancellationRequested)
                return;

            if (downloadTask.IsCompletedSuccessfully)
            {
                _currentCoroutine = StartCoroutine(SyncLocalDatabaseWithWebCoroutine(downloadTask.Result));
            }
            else
            {
                Debug.LogError("Couldn't download score holders");
            }
        }

        private IEnumerator SyncLocalDatabaseWithWebCoroutine(List<ScoreEntry> downloadedScoreEntries)
        {
            _personalResults = JsonSaverLoader.LoadScoreHolders();

            List<ScoreEntry> uploadNewLocalEntriesToWebList = _personalResults.Except(downloadedScoreEntries).ToList();
            List<ScoreEntry> saveNewWebEntriesLocallyList = downloadedScoreEntries.Except(_personalResults).ToList();

            _personalAndWebResults.AddRange(_personalResults);
            _personalAndWebResults.AddRange(saveNewWebEntriesLocallyList);

            foreach (ScoreEntry scoreEntry in uploadNewLocalEntriesToWebList)
            {
                ScoreWebUploader.UploadScoreHolder(scoreEntry.name, scoreEntry.score, false);
                yield return null;
            }

            if(_personalAndWebResults.Count > 0)
                Highscore = _personalAndWebResults.OrderByDescending(entry => entry.score).First().score;

            _currentCoroutine = null;
            DatabaseUpdated = true;
            OnLocalDatabaseUpdated?.Invoke();
        }
    }
}