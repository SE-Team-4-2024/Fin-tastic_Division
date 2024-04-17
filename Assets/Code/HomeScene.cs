using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Threading.Tasks;


public class HomeScene : MonoBehaviour
{
    [SerializeField]
    public Button okayButton, settingsButton;
    [SerializeField]
    private GameObject settingsPanel;
     
    [SerializeField]
    private GameObject backgroundOverlayPanel;
    public void OnButtonClick()
    {
        // Activate the settings panel
        settingsPanel.SetActive(true);
        
        backgroundOverlayPanel.SetActive(true);
    }

    // Start is called before the first frame update
    void Start()
    {
        // StartCoroutine(GetListOfUsers());
        // StartCoroutine(GetorCreateUser());
        // StartCoroutine(UpdateName("johnwick"));
        // StartCoroutine(UpdateSound(false));
        // StartCoroutine(UpdateMusic(true));
        // StartCoroutine(UpdateProfilePicture(1));
        // StartCoroutine(UpdatePrimaryUser("device123546789_gksssss_gowtham"));
        settingsButton.onClick.AddListener(OpenSettingsPanel);
        okayButton.onClick.AddListener(CloseSettingsPanel);
        
    }

    public void OpenSettingsPanel() 
    {
        settingsPanel.SetActive(true);
        backgroundOverlayPanel.SetActive(true);
    }
    public void CloseSettingsPanel() 
    {
        settingsPanel.SetActive(false);
        backgroundOverlayPanel.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
private IEnumerator GetListOfUsers()
{
    string deviceId = SystemInfo.deviceUniqueIdentifier;
    // string deviceId = "gowtham_gowtham";

    // Start the coroutine to fetch users
    IEnumerator coroutine = UserProfile.GetUsers(deviceId, onSuccess, onError);
    yield return StartCoroutine(coroutine);
}


private void onSuccess(User[] users)
{
    Debug.Log(users);
    Debug.Log("Users Data...");
    // Handle the success case here
}

private void onError(string errorMessage)
{
    Debug.LogError("Error fetching user data: " + errorMessage);
    //Need to handle the logic to open the create user panel here..
}

private IEnumerator UpdateName(string name){
    PlayerPrefs.SetString("userID", "device123546789_gksssss_gowtham");
    string userId = PlayerPrefs.GetString("userID");
    Debug.Log("User" +userId);

    yield return StartCoroutine(UserProfile.UpdateName(userId, name,
        // onSuccess callback
        (response) =>
        {
            Debug.Log("Name Updated Successfully" + response);
            // Handle successful creation
        },
        // onError callback
        (errorMessage) =>
        {
            Debug.LogError(errorMessage);
            // Handle error
        }
    ));
}



private IEnumerator UpdateSound(bool isSoundEnabled){
    PlayerPrefs.SetString("userID", "device123546789_gksssss_gowtham");
    string userId = PlayerPrefs.GetString("userID");
    Debug.Log("User Sound Enabled" +isSoundEnabled);
    string isSoundEnabledValue = isSoundEnabled.ToString();


    yield return StartCoroutine(UserProfile.UpdateSound(userId, isSoundEnabledValue,
        // onSuccess callback
        (response) =>
        {
            Debug.Log("Sound Updated Successfully" + response);
            // Handle successful creation
        },
        // onError callback
        (errorMessage) =>
        {
            Debug.LogError(errorMessage);
            // Handle error
        }
    ));
}


private IEnumerator UpdateMusic(bool isMusicEnabled){
    PlayerPrefs.SetString("userID", "device123546789_gks_gowsthamss");
    string userId = PlayerPrefs.GetString("userID");
    Debug.Log("User Music Enabled" +isMusicEnabled);
    string isMusicEnabledValue = isMusicEnabled.ToString();

    yield return StartCoroutine(UserProfile.UpdateMusic(userId, isMusicEnabledValue,
        // onSuccess callback
        (response) =>
        {
            Debug.Log("Music Updated Successfully" + response);
            // Handle successful creation
        },
        // onError callback
        (errorMessage) =>
        {
            Debug.LogError(errorMessage);
            // Handle error
        }
    ));
}


private IEnumerator UpdateProfilePicture(int profilePicture){
    PlayerPrefs.SetString("userID", "device123546789_gksssss_gowtham");
    string userId = PlayerPrefs.GetString("userID");
    Debug.Log("User Profile Picture" +profilePicture);
    string profilePictureValue = profilePicture.ToString();

    yield return StartCoroutine(UserProfile.UpdateProfilePicture(userId, profilePictureValue,
        // onSuccess callback
        (response) =>
        {
            Debug.Log("Profile Picture Updated Successfully" + response);
            // Handle successful creation
        },
        // onError callback
        (errorMessage) =>
        {
            Debug.LogError(errorMessage);
            // Handle error
        }
    ));
}



private IEnumerator UpdatePrimaryUser(string userID){
    PlayerPrefs.SetString("userID", "device123546789_gksssss_gowtham");
    string deviceID = "device123546789_gksssss";

    yield return StartCoroutine(UserProfile.UpdatePrimaryUser(userID,deviceID,
        // onSuccess callback
        (response) =>
        {
            Debug.Log("Profile Picture Updated Successfully" + response);
            // Handle successful creation
        },
        // onError callback
        (errorMessage) =>
        {
            Debug.LogError(errorMessage);
            // Handle error
        }
    ));
}
private IEnumerator GetorCreateUser()
{
    // string deviceId = SystemInfo.deviceUniqueIdentifier;
    string deviceId = "device123546789_gksssss";
    string name = "gowtham";
    string profilePicture = "3";

    yield return StartCoroutine(UserProfile.GetorCreateUser(deviceId, name, profilePicture,
        // onSuccess callback
        (userId) =>
        {
            PlayerPrefs.SetString("userID", userId);
            // Handle successful creation
        },
        // onError callback
        (errorMessage) =>
        {
            Debug.LogError(errorMessage);
            // Handle error
        }
    ));
}




private void onSuccessfulCreation(string userId)
{
    Debug.Log(userId);
    Debug.Log("User id fetched");
    
    // Handle the success case here
}







}
