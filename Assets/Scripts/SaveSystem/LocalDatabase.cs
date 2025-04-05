using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Youregone.SL;
using Youregone.Web;
using Youregone.Utils;
using System.Linq;

namespace Youregone.SaveSystem
{
    public class LocalDatabase : MonoBehaviour, IService
    {
        private List<ScoreEntry> _scoreHoldersList;

        public List<ScoreEntry> ScoreHoldersList => _scoreHoldersList;
        public int Highscore { get; private set; }

        public bool InternetConnectionAvailable { get; private set; }

        private void Awake()
        {
            UpdateLocalDatabase();
        }

        private async void UpdateLocalDatabase()
        {
            if(!await InternetChecker.IsInternetAvailableAsync())
            {
                Debug.Log("No internet connection! Loading from json");
                InternetConnectionAvailable = false;
                _scoreHoldersList = JsonSaverLoader.LoadScoreHolders();
                Highscore = _scoreHoldersList.OrderByDescending(entry => entry.score).FirstOrDefault().score;
                return;
            }

            Debug.Log("Internet connection available! Loading from web");
            InternetConnectionAvailable = true;

            Task<List<ScoreEntry>> downloadTask = ScoreWebUploader.DownloadScoreHoldersAsync();

            await downloadTask;

            if (downloadTask.IsCompletedSuccessfully)
            {
                Debug.Log("Database updated successfully");
                _scoreHoldersList = downloadTask.Result;
                Highscore = downloadTask.Result[0].score;
            }
            else
            {
                Debug.LogError("Couldn't update database");
            }
        }

        public void SaveNewScoreEntry(string name, int score)
        {
            _scoreHoldersList.Add(new ScoreEntry(name, score));
            JsonSaverLoader.SaveScoreHoldersJson(_scoreHoldersList);
        }
    }
}