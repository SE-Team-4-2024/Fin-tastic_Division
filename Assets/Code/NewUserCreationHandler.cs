using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class NewUserCreationHandler : MonoBehaviour
{
    public GameObject newUserAdditionPanel; // Panel on which the new user is created.
    [SerializeField] private Button createButton;
    private InputField[] inputFields; // Array to store references to input fields
   [SerializeField] private Button musicButton, soundButton;
    [SerializeField] private AudioClip clickSound, toggleSound;

    public TMP_InputField tmp_InputField;

    private AudioSource audioSource;
    private AudioController audioController; // Reference to AudioController

    // Global variables to store music and sound settings
    private bool isMusicOn = true;
    private bool isSoundOn = true;


    private void Start()
    {

        // Find the AudioController in the scene
        audioSource = GetComponent<AudioSource>();
        audioController = FindObjectOfType<AudioController>(); 
        inputFields = newUserAdditionPanel.GetComponentsInChildren<InputField>();
        musicButton.onClick.AddListener(ToggleMusic);
        soundButton.onClick.AddListener(ToggleSound);
        createButton.onClick.AddListener(OnCreateButtonClick);
    }

    public void OnCreateButtonClick()
    {
        string name = inputFields[0].text;
        UserManager userManagerInstance = FindObjectOfType<UserManager>();
        int profilePicture =  userManagerInstance.GetImage();
        bool isMusicEnabled = PlayerPrefs.GetInt(UserManager.MUSIC_KEY, 1) == 1;
        bool isSoundEnabled = PlayerPrefs.GetInt(UserManager.SOUND_KEY, 1) == 1;
        StartCoroutine(GetorCreateUser(name, profilePicture.ToString(), isMusicEnabled.ToString(), isSoundEnabled.ToString()));
    }

    private void PlayClickSound()
    {
        if (clickSound != null && audioSource != null && isSoundOn)
        {
            Debug.Log("Playing click sound.");
            audioSource.PlayOneShot(clickSound);
        }
        else
        {
            Debug.Log("Sound is not playing: clickSound=" + clickSound + ", audioSource=" + audioSource + ", isSoundOn=" + isSoundOn);
        }
    }

    private void ToggleMusic()
    {
        audioSource.PlayOneShot(toggleSound);
        isMusicOn = !isMusicOn; // Toggle the music state

        // Save the music setting to PlayerPrefs
        Debug.Log(isMusicOn + "Music On..")
        PlayerPrefs.SetInt(UserManager.MUSIC_KEY, isMusicOn ? 1 : 0);
        PlayerPrefs.Save();

        PlayClickSound();

        // Toggle background music
        audioController.ToggleBackgroundMusic();
    }

    private void ToggleSound()
    {
        audioSource.PlayOneShot(toggleSound);
        isSoundOn = !isSoundOn; // Toggle the sound state

        // Save the sound setting to PlayerPrefs
        Debug.Log("Sound Enabked" + isSoundOn);
        PlayerPrefs.SetInt(UserManager.SOUND_KEY, isSoundOn ? 1 : 0);
        PlayerPrefs.Save();

        PlayClickSound();
    }


    private IEnumerator GetorCreateUser(string name, string profilePicture, string isMusicEnabled, string isSoundEnabled)
    {
        string deviceId = SystemInfo.deviceUniqueIdentifier;
        Debug.Log("[New User Creation Handler] Creating New User For " + name + " " + deviceId);
        yield return StartCoroutine(UserProfile.GetorCreateUser(deviceId, name, profilePicture, isMusicEnabled, isSoundEnabled,
            // onSuccess callback
            (userId) =>
            {   // Saving the name and user id , if api is successful.
                PlayerPrefs.SetString(UserManager.USERID_KEY, userId);
                PlayerPrefs.SetString(UserManager.NAME_KEY, name);
                tmp_InputField.text = name; // Loading the name for settings scene
                PlayerPrefs.Save();
                Debug.Log("[New User Creation Handler] User Id "+ userId);
                newUserAdditionPanel.SetActive(false); // Once passes, the panel is not needed
            },
            // onError callback
            (errorMessage) =>
            {
                Debug.LogError("[New User Creation Handler] Failed to create error " + errorMessage);
            }
        ));
    }


}