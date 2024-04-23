using UnityEngine;
using UnityEngine.UI;

public class PanelUserData : MonoBehaviour
{
    public string userID;
    public string name;


    private void Start()
    {
        // Access the custom values from the panel prefab
        
    }

    public void SetCustomValues(string username, string userid)
    {
        // Set the custom values in the panel prefab
        userID = userid;
        name = username;
        
    }

    public string GetUserID()
    {
        return userID;
    }

    public string GetUserName()
    {
        return name;
    }
}