

using System.Collections.Generic;

[System.Serializable]
public class ScoreInfo 
{
    public int score;
    public ulong id;
    public string name;
}

[System.Serializable] 
public class PlayerScores
{
    public List<ScoreInfo> _scores;
}
