using UnityEngine;
using UnityEngine.Networking;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Diagnostics;

namespace Youregone.Web
{
    public static partial class ScoreWebUploader
    {
        private const string URL = "https://script.google.com/macros/s/AKfycbzEEgDe37oqb2v2acMjeFS9IXocM5f58tNuko01EP0icEbp2rO0K2P_dNOKBMhId9YN/exec";
        private const string AMOUNT_OF_TOP_RECORD_HOLDERS = "topAmount=8";

        public static void UploadScoreHolder(string playerName, int playerScore)
        {
            ScoreEntry entry = new(playerName, playerScore);

            UnityEngine.Debug.Log($"Posting {JsonUtility.ToJson(entry)}");
            _= SendDataAsync(JsonUtility.ToJson(entry));
        }

        public static async Task<List<ScoreEntry>> DownloadScoreHoldersAsync()
        {
            try
            {
                using (UnityWebRequest www = UnityWebRequest.Get(URL))
                {
                    Stopwatch stopwatch = new();
                    stopwatch.Start();

                    UnityWebRequestAsyncOperation operation = www.SendWebRequest();

                    while (!operation.isDone)
                        await Task.Yield();

                    stopwatch.Stop();

                    if (www.result == UnityWebRequest.Result.Success)
                    {
                        string rawJson = "{\"scoreEntryList\":" + www.downloadHandler.text + "}";
                        UnityEngine.Debug.Log($"Received JSON (Time spent: {stopwatch.ElapsedMilliseconds}ms): " + rawJson);

                        ScoreEntryList scores = JsonUtility.FromJson<ScoreEntryList>(rawJson);

                        if (scores == null || scores.scoreEntryList == null)
                        {
                            UnityEngine.Debug.LogError("Failed to parse scores. Data is null.");
                            return null;
                        }

                        return scores.scoreEntryList.OrderByDescending(entry => entry.score).ToList();
                    }
                    else
                    {
                        UnityEngine.Debug.LogError("Failed to download scores: " + www.error);
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError("Error during download: " + ex.Message);
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
                    UnityEngine.Debug.Log("Score uploaded!");
                }
                else
                {
                    UnityEngine.Debug.LogError("Error uploading score: " + www.error);
                }
            }
        }
    }
}