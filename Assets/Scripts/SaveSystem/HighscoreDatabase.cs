using System.Collections.Generic;
using System.Linq;

namespace Youregone.SaveSystem
{
    public class HighscoreDatabase
    {
        private SerializableDictionary<string, int> _nameHighscoreDictionary;

        public Dictionary<string, int> NameHighScoreDictionary => _nameHighscoreDictionary;

        public HighscoreDatabase()
        {
            _nameHighscoreDictionary = JsonSaverLoader.LoadScoreHolders();
        }

        public int GetHighestScore()
        {
            return _nameHighscoreDictionary.OrderByDescending(entry => entry.Value).FirstOrDefault().Value;
        }

        public Dictionary<string, int> GetHighscoreHolders()
        {
            return _nameHighscoreDictionary;
        }

        public void SaveScoreHolder(string name, int score)
        {
            if(_nameHighscoreDictionary.ContainsKey(name))
            {
                if (_nameHighscoreDictionary[name] > score)
                    _nameHighscoreDictionary[name] = score;
                else
                    return;
            }
            else
            {
                _nameHighscoreDictionary.Add(name, score);
            }

            JsonSaverLoader.SaveScoreHolder(_nameHighscoreDictionary);
        }
    }
}