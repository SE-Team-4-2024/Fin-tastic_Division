using UnityEngine;

[System.Serializable]
public class User
{
    public string UserID;
    public string Name;
    public bool IsSoundEnabled;
    public bool IsMusicEnabled;
    public int Age;
    public string Password;
    public string DeviceID;

    public string isPrimaryUser;

    public string profilePicture;
}

[System.Serializable]
public class UserContent
{
    public User[] users;
}