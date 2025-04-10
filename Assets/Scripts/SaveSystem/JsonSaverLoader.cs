using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Youregone.SaveSystem
{
    public static class JsonSaverLoader
    {
        private const string SAVE_DIRECTORY_NAME = "Save Files";

        private static string _oldFilePath = Path.Combine(Application.persistentDataPath, "scoreData.json");
        private static string _newFilePath = Path.Combine(Application.persistentDataPath, "saveData.json");

        private static readonly string CurrentRecordHoldersFileName = "saveData.json";
        private static readonly string SaveDirectoryPath = Path.Combine(Application.persistentDataPath, SAVE_DIRECTORY_NAME);
        private static readonly string CurrentFilePath = Path.Combine(SaveDirectoryPath, CurrentRecordHoldersFileName);

        public static void SaveScoreHoldersJson(List<ScoreEntry> scoreHolders)
        {
            EnsureSaveDirectoryExists();
            DeleteOldJsonFiles();
            DeleteLegacySaveFiles();

            ScoreEntryList scoreEntryList = new(scoreHolders);
            string json = JsonUtility.ToJson(scoreEntryList, true);

            File.WriteAllText(CurrentFilePath, json);
        }

        public static List<ScoreEntry> LoadScoreHolders()
        {
            EnsureSaveDirectoryExists();
            DeleteOldJsonFiles();
            DeleteLegacySaveFiles();

            if (!File.Exists(CurrentFilePath))
            {
                Debug.LogWarning("Save file does not exist");
                return new List<ScoreEntry>();
            }

            string json = File.ReadAllText(CurrentFilePath);
            ScoreEntryList scoreEntryList = JsonUtility.FromJson<ScoreEntryList>(json);

            return scoreEntryList.list;
        }

        public static void DeleteLegacySaveFiles()
        {
            if (File.Exists(_oldFilePath))
                File.Delete(_oldFilePath);

            if (File.Exists(_newFilePath))
                File.Delete(_newFilePath);
        }

        public static void DeleteOldJsonFiles()
        {
            EnsureSaveDirectoryExists();

            string[] jsonFiles = Directory.GetFiles(SaveDirectoryPath, "*.json");
            foreach (string file in jsonFiles)
            {
                if (!file.Equals(CurrentFilePath))
                {
                    File.Delete(file);
                }
            }
        }

        private static void EnsureSaveDirectoryExists()
        {
            if (!Directory.Exists(SaveDirectoryPath))
            {
                Directory.CreateDirectory(SaveDirectoryPath);
            }
        }
    }
}