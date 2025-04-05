using System.Collections.Generic;

[System.Serializable]
public class ScoreEntryList
{
    public List<ScoreEntry> scoreEntryList;

    public ScoreEntryList(List<ScoreEntry> scoreEntryList)
    {
        this.scoreEntryList = scoreEntryList;
    }
}