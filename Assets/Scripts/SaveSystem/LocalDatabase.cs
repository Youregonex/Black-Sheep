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

        public void SaveNewScoreEntry(string name, int score)
        {
            _localScoreHoldersList.Add(new ScoreEntry(name, score));
            JsonSaverLoader.SaveScoreHoldersJson(_localScoreHoldersList);
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
                else
                    Highscore = 0;

                return;
            }

            Debug.Log("Internet connection available! Loading from web");
            InternetConnectionAvailable = true;

            Task<List<ScoreEntry>> downloadTask = ScoreWebUploader.DownloadScoreHoldersAsync();

            await downloadTask;

            if (downloadTask.IsCompletedSuccessfully)
            {
                StartCoroutine(SyncLocalDatabaseWithWebCoroutine(downloadTask.Result));
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
        }
    }
}