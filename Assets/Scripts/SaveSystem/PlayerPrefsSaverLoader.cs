using UnityEngine;
using Youregone.SL;

namespace Youregone.SaveSystem
{
    public class PlayerPrefsSaverLoader : MonoBehaviour, IService
    {
        private const string OUTRO_ENABLED_PLAYER_PREFS_KEY = "OUTROENABLED";
        public const string PLAYERPREFS_HIGH_SCORE_KEY = "HIGHSCORE";

        public bool OutroEnabled
        {
            get
            {
                if (!PlayerPrefs.HasKey(OUTRO_ENABLED_PLAYER_PREFS_KEY))
                    return true;

                if(PlayerPrefs.GetInt(OUTRO_ENABLED_PLAYER_PREFS_KEY) == 0)
                    return false;

                return true;
            }
        }

        private void Awake()
        {
            if(!PlayerPrefs.HasKey(OUTRO_ENABLED_PLAYER_PREFS_KEY))
                PlayerPrefs.SetInt(OUTRO_ENABLED_PLAYER_PREFS_KEY, 1);
        }

        public void SaveHighScore(int highscore)
        {
            Debug.Log("Saving High Score");
            PlayerPrefs.SetInt(PLAYERPREFS_HIGH_SCORE_KEY, highscore);
            PlayerPrefs.Save();
        }

        public int GetHighScore()
        {
            Debug.Log($"Loading High Score {PlayerPrefs.GetInt(PLAYERPREFS_HIGH_SCORE_KEY)}");
            return PlayerPrefs.GetInt(PLAYERPREFS_HIGH_SCORE_KEY);
        }

        public void ToggleOutroEnable()
        {
            if(PlayerPrefs.GetInt(OUTRO_ENABLED_PLAYER_PREFS_KEY) == 0)
            {
                PlayerPrefs.SetInt(OUTRO_ENABLED_PLAYER_PREFS_KEY, 1);
                PlayerPrefs.Save();
            }
            else
            {
                PlayerPrefs.SetInt(OUTRO_ENABLED_PLAYER_PREFS_KEY, 0);
                PlayerPrefs.Save();
            }
        }
    }
}