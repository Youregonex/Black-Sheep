using UnityEngine;
using Youregone.SL;

namespace Youregone.SaveSystem
{
    public class PlayerPrefsSaverLoader : MonoBehaviour, IService
    {
        private const string OUTRO_ENABLED_PLAYER_PREFS_KEY = "OUTROENABLED";
        private const string LAST_RECORDHOLDER_NAME = "LASTRECORHOLDERNAME";
        private const string PLAYER_CLOVERS_COLLECTED = "PLAYERCLOVERSCOLLECTED";
        private const string PLAYER_RARE_CLOVERS_COLLECTED = "PLAYERRARECLOVERSCOLLECTED";

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

        public string GetLastRecordHolderName()
        {
            if (!PlayerPrefs.HasKey(LAST_RECORDHOLDER_NAME))
                return null;

            return PlayerPrefs.GetString(LAST_RECORDHOLDER_NAME);
        }

        public void SaveLastRecordHolderName(string name)
        {
            if (PlayerPrefs.HasKey(LAST_RECORDHOLDER_NAME) && PlayerPrefs.GetString(LAST_RECORDHOLDER_NAME) == name)
                return;

            Debug.Log($"Saving last record holder: {name}");
            PlayerPrefs.SetString(LAST_RECORDHOLDER_NAME, name);
            PlayerPrefs.Save();
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


        public void SavePlayerClovers(int baseCloversCollectedAmount, int rareCloversCollectedAmount)
        {
            int baseCloversTosave;
            int rareCloversToSave;

            if (PlayerPrefs.HasKey(PLAYER_CLOVERS_COLLECTED))
                baseCloversTosave = baseCloversCollectedAmount + PlayerPrefs.GetInt(PLAYER_CLOVERS_COLLECTED);
            else
                baseCloversTosave = baseCloversCollectedAmount;

            if (PlayerPrefs.HasKey(PLAYER_RARE_CLOVERS_COLLECTED))
                rareCloversToSave = rareCloversCollectedAmount + PlayerPrefs.GetInt(PLAYER_RARE_CLOVERS_COLLECTED);
            else
                rareCloversToSave = rareCloversCollectedAmount;

            PlayerPrefs.SetInt(PLAYER_CLOVERS_COLLECTED, baseCloversTosave);
            PlayerPrefs.SetInt(PLAYER_RARE_CLOVERS_COLLECTED, rareCloversToSave);

            PlayerPrefs.Save();
        }

        public int GetBaseCloverAmount()
        {
            if (PlayerPrefs.HasKey(PLAYER_CLOVERS_COLLECTED))
                return PlayerPrefs.GetInt(PLAYER_CLOVERS_COLLECTED);

            return 0;
        }

        public int GetRareCloverAmount()
        {
            if (PlayerPrefs.HasKey(PLAYER_RARE_CLOVERS_COLLECTED))
                return PlayerPrefs.GetInt(PLAYER_RARE_CLOVERS_COLLECTED);

            return 0;
        }
    }
}