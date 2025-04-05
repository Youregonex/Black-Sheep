using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Youregone.SL;
using Youregone.Web;

namespace Youregone.SaveSystem
{
    public class HighscoreDatabase : MonoBehaviour, IService
    {
        private SerializableDictionary<string, int> _scoreHoldersDictionary;

        public Dictionary<string, int> ScoreHoldersDictionary => _scoreHoldersDictionary;
        public int Highscore { get; private set; }

        private void Awake()
        {
            UpdateHighscore();
        }

        private async void UpdateHighscore()
        {
            Task<Dictionary<string, int>> downloadTask = ScoreWebUploader.DownloadScoreHoldersAsync();

            await downloadTask;

            if (downloadTask.IsCompletedSuccessfully)
            {
                Debug.Log("Highscore successfully updated!");
                Highscore = downloadTask.Result.ElementAt(0).Value;
            }
            else
            {
                Debug.LogError("Couldn't update highscore");
            }
        }
    }
}