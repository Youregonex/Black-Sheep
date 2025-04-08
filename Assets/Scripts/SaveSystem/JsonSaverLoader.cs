using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Youregone.SaveSystem
{
    public static class JsonSaverLoader
    {
        private static string _oldFilePath = Path.Combine(Application.persistentDataPath, "scoreData.json");
        private static string _newFilePath = Path.Combine(Application.persistentDataPath, "saveData.json");

        public static void SaveScoreHoldersJson(List<ScoreEntry> scoreHolders)
        {
            ScoreEntryList scoreEntryList = new(scoreHolders);

            string json = JsonUtility.ToJson(scoreEntryList, true);
            File.WriteAllText(_newFilePath, json);
            Debug.Log($"Data saved at {_newFilePath}");
        }

        public static List<ScoreEntry> LoadScoreHolders()
        {
            if (!File.Exists(_newFilePath))
            {
                Debug.LogWarning("Save file does not exist");
                return new List<ScoreEntry>();
            }

            string json = File.ReadAllText(_newFilePath);
            ScoreEntryList scoreEntryList = JsonUtility.FromJson<ScoreEntryList>(json);

            return scoreEntryList.list;
        }

        public static void DeleteOldScoreFileJson()
        {
            if (File.Exists(_oldFilePath))
            {
                File.Delete(_oldFilePath);
                Debug.Log("Save file deleted: " + _oldFilePath);
            }
            else
            {
                Debug.LogWarning("There is no save file at: " + _oldFilePath);
            }
        }
    }
}