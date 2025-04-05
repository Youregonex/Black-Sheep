using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace Youregone.Web
{
    public static class ScoreWebUploader
    {
        private const string URL = "https://script.google.com/macros/s/AKfycbwuwVTi1YJvVCmOHUzecIgouw961cfemhwV50wzRs0vfcMdc4fwLYL3ePtCeMpWInnX/exec";

        [System.Serializable]
        public class ScoreEntry
        {
            public string name;
            public int score;
        }

        [System.Serializable]
        public class ScoreEntryList
        {
            public List<ScoreEntry> list;
        }

        public static void UploadScoreHolder(string playerName, int playerScore, MonoBehaviour coroutineHost)
        {
            ScoreEntry entry = new()
            {
                name = playerName,
                score = playerScore
            };

            Debug.Log($"Posting {JsonUtility.ToJson(entry)}");
            coroutineHost.StartCoroutine(SendData(JsonUtility.ToJson(entry)));
        }

        public static async Task<Dictionary<string, int>> DownloadScoreHoldersAsync()
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
                        string rawJson = "{\"list\":" + www.downloadHandler.text + "}";
                        Debug.Log("Received JSON: " + rawJson);

                        var scores = JsonUtility.FromJson<ScoreEntryList>(rawJson);

                        if (scores == null || scores.list == null)
                        {
                            Debug.LogError("Failed to parse scores. Data is null.");
                            return null;
                        }

                        Dictionary<string, int> scoreDict = new();
                        foreach (var entry in scores.list)
                        {
                            if (!scoreDict.ContainsKey(entry.name))
                                scoreDict[entry.name] = entry.score;
                        }

                        return scoreDict;
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

        private static IEnumerator SendData(string json)
        {
            UnityWebRequest www = UnityWebRequest.PostWwwForm(URL, "");
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Score sent!");
            }
            else
            {
                Debug.LogError("Error: " + www.error);
            }
        }
    }
}