using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Youregone.SaveSystem
{
    public static class JsonSaverLoader
    {
        private static string _filePath = Path.Combine(Application.persistentDataPath, "scoreData.json");

        public static void SaveScoreHoldersJson(List<ScoreEntry> scoreHolders)
        {
            ScoreEntryList scoreEntryList = new(scoreHolders);

            string json = JsonUtility.ToJson(scoreEntryList, true);
            File.WriteAllText(_filePath, json);
            Debug.Log($"Data saved at {_filePath}");
        }

        public static List<ScoreEntry> LoadScoreHolders()
        {
            if (!File.Exists(_filePath))
            {
                Debug.LogWarning("Save file does not exist");
                return new List<ScoreEntry>();
            }

            string json = File.ReadAllText(_filePath);
            ScoreEntryList scoreEntryList = JsonUtility.FromJson<ScoreEntryList>(json);

            return scoreEntryList.scoreEntryList;
        }

        public static void DeleteScoreFileJson()
        {
            if (File.Exists(_filePath))
            {
                File.Delete(_filePath);
                Debug.Log("Save file deleted: " + _filePath);
            }
            else
            {
                Debug.LogWarning("There is no save file at: " + _filePath);
            }
        }
    }
}