using System.Collections.Generic;

[System.Serializable]
public class ScoreEntryList
{
    public List<ScoreEntry> list;

    public ScoreEntryList(List<ScoreEntry> scoreEntryList)
    {
        this.list = scoreEntryList;
    }
}