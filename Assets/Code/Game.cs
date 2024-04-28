using UnityEngine;

[System.Serializable]
public class Game
{
    public string gameID;
    public string userID;
    public int noOfCorrectAnswers;
    public int noOfWrongAnswers;
    public bool gameCompleted;
    public double accuracyRate;
    public double completionRate;
}

[System.Serializable]
public class GameContent
{
    public Game[] games;
}