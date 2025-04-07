using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Youregone.SL;
using Youregone.Web;
using Youregone.Utils;
using System.Linq;
using System.Collections;
using System.Threading;

namespace Youregone.SaveSystem
{
    public class LocalDatabase : MonoBehaviour, IService
    {
        private List<ScoreEntry> _localScoreHoldersList = new();

        public List<ScoreEntry> ScoreHoldersList => _localScoreHoldersList.OrderByDescending(entry => entry.score).ToList();
        public int Highscore { get; private set; }

        public bool InternetConnectionAvailable { get; private set; }

        private Coroutine _currentCoroutine;
        private CancellationTokenSource _cts;

        private void Awake()
        {
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

        public void SaveNewScoreEntry(string name, int score)
        {
            ScoreEntry scoreEntry = new(name, score);

            if (_localScoreHoldersList.Contains(scoreEntry))
                return;

            _localScoreHoldersList.Add(scoreEntry);
            JsonSaverLoader.SaveScoreHoldersJson(_localScoreHoldersList);
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
                _localScoreHoldersList = JsonSaverLoader.LoadScoreHolders();

                if (_localScoreHoldersList.Count > 0)
                    Highscore = _localScoreHoldersList.OrderByDescending(entry => entry.score).FirstOrDefault().score;
                else
                    Highscore = 0;

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
            _localScoreHoldersList = JsonSaverLoader.LoadScoreHolders();

            List<ScoreEntry> uploadNewLocalEntriesToWebList = _localScoreHoldersList.Except(downloadedScoreEntries).ToList();
            List<ScoreEntry> saveNewWebEntriesLocallyList = downloadedScoreEntries.Except(_localScoreHoldersList).ToList();

            if(saveNewWebEntriesLocallyList.Count > 0)
            {
                _localScoreHoldersList.AddRange(saveNewWebEntriesLocallyList);
                JsonSaverLoader.SaveScoreHoldersJson(_localScoreHoldersList);
            }

            foreach (ScoreEntry scoreEntry in uploadNewLocalEntriesToWebList)
            {
                ScoreWebUploader.UploadScoreHolder(scoreEntry.name, scoreEntry.score);
                yield return null;
            }

            if (_localScoreHoldersList.Count == 0)
                Highscore = 0;
            else
                Highscore = _localScoreHoldersList.OrderByDescending(entry => entry.score).First().score;

            _currentCoroutine = null;
        }
    }
}