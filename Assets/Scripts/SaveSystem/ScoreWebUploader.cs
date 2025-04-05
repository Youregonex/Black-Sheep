using UnityEngine;
using UnityEngine.Networking;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace Youregone.Web
{
    public static partial class ScoreWebUploader
    {
        private const string URL = "https://script.google.com/macros/s/AKfycbymtSHB_9x5UosYnFxvS9NwHGrgwaraXH0-pif9q2CO7jsRnnaLombl3kYMMt9FdRKB/exec";

        public static void UploadScoreHolder(string playerName, int playerScore)
        {
            ScoreEntry entry = new(playerName, playerScore);

            Debug.Log($"Posting {JsonUtility.ToJson(entry)}");
            _= SendDataAsync(JsonUtility.ToJson(entry));
        }

        public static async Task<List<ScoreEntry>> DownloadScoreHoldersAsync()
        {
            try
            {
                using (UnityWebRequest www = UnityWebRequest.Get(URL))
                {
                    var operation = www.SendWebRequest();

                    while (!operation.isDone)
                        await Task.Yield();

                    if (www.result == UnityWebRequest.Result.Success)
                    { 
                        string rawJson = "{\"scoreEntryList\":" + www.downloadHandler.text + "}";
                        Debug.Log("Received JSON: " + rawJson);

                        var scores = JsonUtility.FromJson<ScoreEntryList>(rawJson);

                        if (scores == null || scores.scoreEntryList == null)
                        {
                            Debug.LogError("Failed to parse scores. Data is null.");
                            return null;
                        }

                        return scores.scoreEntryList;
                    }
                    else
                    {
                        Debug.LogError("Failed to download scores: " + www.error);
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Error during download: " + ex.Message);
                return null;
            }
        }

        public static async Task SendDataAsync(string json)
        {
            using (UnityWebRequest www = new(URL, UnityWebRequest.kHttpVerbPOST))
            {
                byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
                www.uploadHandler = new UploadHandlerRaw(bodyRaw);
                www.downloadHandler = new DownloadHandlerBuffer();
                www.SetRequestHeader("Content-Type", "application/json");

                var operation = www.SendWebRequest();

                while (!operation.isDone)
                    await Task.Yield();

                if (www.result == UnityWebRequest.Result.Success)
                {
                    Debug.Log("Score uploaded!");
                }
                else
                {
                    Debug.LogError("Error uploading score: " + www.error);
                }
            }
        }
    }
}