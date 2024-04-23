using UnityEngine;
using System.Collections;


/**
 * File: UserManager.cs
 * Purpose: Fetches the list of users associated with the device ID and stores common attributes under the game object.
 */

public class UserManager : MonoBehaviour
{
    private string userID;
    private string userName;
    private User[] users;

    private string editUserID; // To contain the user id of the panel edited.

    void Start()
    {
        FetchUsersSynchronously();
    }

    public User[] FetchUsersSynchronously()
    {
        string deviceId = SystemInfo.deviceUniqueIdentifier;
        User[] fetchedUsers = UserProfile.GetUsersSync(deviceId); // Assuming there's a synchronous method for fetching users
        if (fetchedUsers != null && fetchedUsers.Length >= 1)
        {
            Debug.Log("[User Manager] Found " + fetchedUsers.Length + " users");
            users = fetchedUsers;
        }
        else
        {
            Debug.Log("[User Manager] No users found.");
            users = new User[0]; // Assign an empty array if no users are found
        }


        return users;
    }


    public void SetEditUserID(string edituserID){
        editUserID = edituserID;
    }

    public string GetEditUserID(){
        return editUserID;
    }


    public void SetUserID(string newUserID)
    {
        userID = newUserID;
    }

    // Method to get the user ID
    public string GetUserID()
    {
        return userID;
    }

    // Method to get the name
    public string GetName()
    {
        return userName;
    }

    // Method to get the array of users
    public User[] GetUsers()
    {
        User[] users = FetchUsersSynchronously();
        return users;
    }

    public void Name(string name)
    {
        userName = name;
    }
}
