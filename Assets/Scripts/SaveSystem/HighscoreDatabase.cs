using System.Collections.Generic;
using System.Linq;

namespace Youregone.SaveSystem
{
    public class HighscoreDatabase
    {
        private SerializableDictionary<string, int> _scoreHoldersDictionary;

        public Dictionary<string, int> ScoreHoldersDictionary => _scoreHoldersDictionary;

        public HighscoreDatabase()
        {
            _scoreHoldersDictionary = JsonSaverLoader.LoadScoreHolders();
        }

        public int GetHighestScore()
        {
            return _scoreHoldersDictionary.OrderByDescending(entry => entry.Value).FirstOrDefault().Value;
        }

        public Dictionary<string, int> GetHighscoreHolders()
        {
            return _scoreHoldersDictionary;
        }

        public Dictionary<string, int> GetTopScoreHolders(int amount)
        {
            Dictionary<string, int> topScores = _scoreHoldersDictionary
                .OrderByDescending(entry => entry.Value)
                .Take(amount)
                .ToDictionary(entry => entry.Key, entry => entry.Value);

            return topScores;
        }

        public void SaveScoreHolder(string name, int score)
        {
            if(_scoreHoldersDictionary.ContainsKey(name))
            {
                if (_scoreHoldersDictionary[name] < score)
                    _scoreHoldersDictionary[name] = score;
                else
                    return;
            }
            else
            {
                _scoreHoldersDictionary.Add(name, score);
            }

            JsonSaverLoader.SaveScoreHolder(_scoreHoldersDictionary);
        }
    }
}