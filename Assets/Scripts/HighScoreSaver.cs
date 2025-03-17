using UnityEngine;
using Youregone.SL;

namespace Youregone.SaveSystem
{
    public  class HighScoreSaver : MonoBehaviour, IService
    {
        public const string PLAYERPREFS_HIGH_SCORE_KEY = "HIGHSCORE";


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