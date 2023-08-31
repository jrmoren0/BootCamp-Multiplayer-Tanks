[System.Serializable]
public class User
{
    public string username;

    public string password;

    public int highscore;
}

[System.Serializable]

public class HighScores
{
    public User[] highscorers;
}
