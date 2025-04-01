using UnityEngine;
using Youregone.SL;

namespace Youregone.SaveSystem
{
    public class PlayerPrefsSaverLoader : MonoBehaviour, IService
    {
        private const string OUTRO_ENABLED_PLAYER_PREFS_KEY = "OUTROENABLED";

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