using System;

[Serializable]
public class ScoreEntry
{
    public string name;
    public int score;

    public ScoreEntry(string name, int score)
    {
        this.name = name;
        this.score = score;
    }

    public override bool Equals(object obj)
    {
        return obj is ScoreEntry entry &&
               name == entry.name &&
               score == entry.score;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(name, score);
    }
}
