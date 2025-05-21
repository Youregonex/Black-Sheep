using UnityEngine;
using UnityEngine.Networking;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Diagnostics;
using Youregone.Utils;

namespace Youregone.Web
{
    public static partial class ScoreWebUploader
    {
        private const string URL = "https://script.google.com/macros/s/AKfycbwX7S4iOQaxb2yBHHRc6o0Cgs8zqQ3Nw27AYJcuazfNOWia1RAs5KJGp6eU68mlA4b6/exec";
        private const string AMOUNT_OF_TOP_RECORD_HOLDERS = "topAmount=8";

        public static void UploadScoreHolder(string name, int playerScore, bool isShortName)
        {
            ScoreEntry entry;

            if (isShortName)
            {
                string fullName = StringEncryptor.GetFullName(name);
                entry = new(fullName, playerScore);
            }
            else
                entry = new(name, playerScore);
            

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
                        string rawJson = "{\"list\":" + www.downloadHandler.text + "}";
                        UnityEngine.Debug.Log($"Received JSON (Time spent: {stopwatch.ElapsedMilliseconds}ms): " + rawJson);

                        ScoreEntryList scores = JsonUtility.FromJson<ScoreEntryList>(rawJson);

                        if (scores == null || scores.list == null)
                        {
                            UnityEngine.Debug.LogError("Failed to parse scores. Data is null.");
                            return null;
                        }

                        return scores.list.OrderByDescending(entry => entry.score).ToList();
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

        private static async Task SendDataAsync(string json)
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