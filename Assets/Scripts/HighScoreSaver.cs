using UnityEngine;

namespace Youregone.HighScore
{
    public  class HighScoreSaver : MonoBehaviour
    {
        public static HighScoreSaver instance;

        public const string PLAYERPREFS_HIGH_SCORE_KEY = "HIGHSCORE";

        private void Awake()
        {
            instance = this;
        }

        public void SaveHighScore(int highscore)
        {
            Debug.Log("Saving High Score");
            PlayerPrefs.SetInt(PLAYERPREFS_HIGH_SCORE_KEY, highscore);
        }

        public int GetHighScore()
        {
            Debug.Log($"Loading High Score {PlayerPrefs.GetInt(PLAYERPREFS_HIGH_SCORE_KEY)}");
            return PlayerPrefs.GetInt(PLAYERPREFS_HIGH_SCORE_KEY);
        }
    }
}