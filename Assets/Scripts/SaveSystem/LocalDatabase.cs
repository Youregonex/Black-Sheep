using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Youregone.SL;
using Youregone.Web;
using Youregone.Utils;
using System.Linq;
using System.Collections;

namespace Youregone.SaveSystem
{
    public class LocalDatabase : MonoBehaviour, IService
    {
        private List<ScoreEntry> _localScoreHoldersList = new();

        public List<ScoreEntry> ScoreHoldersList => _localScoreHoldersList.OrderByDescending(entry => entry.score).ToList();
        public int Highscore { get; private set; }

        public bool InternetConnectionAvailable { get; private set; }

        private void Awake()
        {
            UpdateLocalDatabaseAsync();
        }

        private async void UpdateLocalDatabaseAsync()
        {
            if(!await InternetChecker.IsInternetAvailableAsync())
            {
                Debug.Log("No internet connection! Loading from json");
                InternetConnectionAvailable = false;
                _localScoreHoldersList = JsonSaverLoader.LoadScoreHolders();

                if (_localScoreHoldersList.Count > 0)
                    Highscore = _localScoreHoldersList.OrderByDescending(entry => entry.score).FirstOrDefault().score;
                else Highscore = 0;

                return;
            }

            Debug.Log("Internet connection available! Loading from web");
            InternetConnectionAvailable = true;

            Task<List<ScoreEntry>> downloadTask = ScoreWebUploader.DownloadScoreHoldersAsync();

            await downloadTask;

            if (downloadTask.IsCompletedSuccessfully)
            {
                Debug.Log("Score holders downloaded");
                StartCoroutine(SyncLocalDatabaseCoroutine(downloadTask.Result));
            }
            else
            {
                Debug.LogError("Couldn't download score holders");
            }
        }

        private IEnumerator SyncLocalDatabaseCoroutine(List<ScoreEntry> downloadedScoreEntries)
        {
            _localScoreHoldersList = JsonSaverLoader.LoadScoreHolders();

            if (_localScoreHoldersList.Count == 0)
                yield break;

            List<ScoreEntry> localListExceptWebList = _localScoreHoldersList.Except(downloadedScoreEntries).ToList();
            List<ScoreEntry> webListExceptLocalList = downloadedScoreEntries.Except(_localScoreHoldersList).ToList();

            _localScoreHoldersList.AddRange(webListExceptLocalList);
            JsonSaverLoader.SaveScoreHoldersJson(_localScoreHoldersList);

            foreach (ScoreEntry scoreEntry in localListExceptWebList)
            {
                ScoreWebUploader.UploadScoreHolder(scoreEntry.name, scoreEntry.score);
                yield return null;
            }

            Highscore = _localScoreHoldersList.OrderByDescending(entry => entry.score).First().score;
        }

        public void SaveNewScoreEntry(string name, int score)
        {
            _localScoreHoldersList.Add(new ScoreEntry(name, score));
            JsonSaverLoader.SaveScoreHoldersJson(_localScoreHoldersList);
        }
    }
}