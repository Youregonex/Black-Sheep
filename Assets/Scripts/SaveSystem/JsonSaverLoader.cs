using System.IO;
using UnityEngine;

namespace Youregone.SaveSystem
{
    public static class JsonSaverLoader
    {
        private static string _filePath = Path.Combine(Application.persistentDataPath, "scoreData.json");

        public static void SaveScoreHolder(SerializableDictionary<string, int> scoreHolders)
        {
            string json = JsonUtility.ToJson(scoreHolders, true);
            File.WriteAllText(_filePath, json);
            Debug.Log($"Data saved at {_filePath}");
        }

        public static SerializableDictionary<string, int> LoadScoreHolders()
        {
            if (!File.Exists(_filePath))
            {
                Debug.LogWarning("Save file does not exist");
                return new SerializableDictionary<string, int>();
            }

            string json = File.ReadAllText(_filePath);
            return JsonUtility.FromJson<SerializableDictionary<string, int>>(json);
        }

        public static void DeleteScoreFile()
        {
            if (File.Exists(_filePath))
            {
                File.Delete(_filePath);
                Debug.Log("File deleted: " + _filePath);
            }
            else
            {
                Debug.LogWarning("File wasn't found: " + _filePath);
            }
        }
    }
}