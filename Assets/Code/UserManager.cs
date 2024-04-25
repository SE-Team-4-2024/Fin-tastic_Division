using UnityEngine;
using System.Collections;
using System;


/**
 * File: UserManager.cs
 * Purpose: Fetches the list of users associated with the device ID and stores common attributes under the game object.
 */

using System.Collections.Generic;

public class UserManager : MonoBehaviour
{
    private string userID;
    private string userName;
    private User[] users;

    private int image;

    public const string USERID_KEY = "userID";
    public const string NAME_KEY = "name";
    public const string MUSIC_KEY = "MusicOn";
    public const string SOUND_KEY = "SoundOn";

    public const string GAME_KEY = "gameID";

    public const string IMAGE_KEY = "image";

    private string[] ALLOWED_IMAGES = { "pfImage1", "pfImage2", "pfImage3", "defaultImage" };

    private GameObject instantiatedPanels;

    private string editUserID; // To contain the user id of the panel edited.

    void Start()
    {
        FetchUsersSynchronously();
    }

    public bool MatchString(string input)
    {
        // Check if the input string matches any string in the array
        foreach (string str in ALLOWED_IMAGES)
        {
            if (input.Equals(str, StringComparison.OrdinalIgnoreCase))
            {
                return true; // Exit the method once a match is found
            }
        }

        return false;
    }

    public User[] FetchUsersSynchronously()
    {
        string deviceId = SystemInfo.deviceUniqueIdentifier;
        User[] fetchedUsers = UserProfile.GetUsersSync(deviceId); // Assuming there's a synchronous method for fetching users
        if (fetchedUsers != null && fetchedUsers.Length >= 1)
        {
            User user = fetchedUsers[0];
            if (user.IsPrimaryUser == "true")
            {
                PlayerPrefs.SetString(USERID_KEY, user.UserID);
                PlayerPrefs.SetInt(SOUND_KEY, user.IsSoundEnabled ? 1 : 0);
                PlayerPrefs.SetInt(MUSIC_KEY, user.IsMusicEnabled ? 1 : 0);
                PlayerPrefs.SetString(NAME_KEY, user.Name);
                PlayerPrefs.Save();
            }
            // Do something with the user object
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


    public int GetImageNo(string imageName)
    {
        int imageNo;

        switch (imageName)
        {
            case "pfImage1":
                imageNo = 2;
                break;
            case "pfImage2":
                imageNo = 3;
                break;
            case "pfImage3":
                imageNo = 4;
                break;
            default:
                imageNo = 1;
                break;
        }
        PlayerPrefs.SetInt(IMAGE_KEY, imageNo);
        PlayerPrefs.Save();

        return imageNo;
    }

    public string GetImageName(int imageNo)
    {
        string imageName;

        switch (imageNo)
        {
            case 2:
                imageName = "pfImage1";
                break;
            case 3:
                imageName = "pfImage2";
                break;
            case 4:
                imageName = "pfImage3";
                break;
            default:
                imageName = "defaultImage";
                break;
        }
        return imageName;
    }


    public int GetImage()
    {
        if (image == null || image == 0)
        {
            return 1;
        }
        return image;
    }


    public void SetImage(int imageKey){
        image = imageKey;
    }
}
