using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class HomeScene : MonoBehaviour
{
    [SerializeField] private Button okayButton, settingsButton, closeButton, musicButton, soundButton, playButton;
    [SerializeField] private Sprite musicOnSprite, musicOffSprite; // Add images for music off states
    [SerializeField] private Sprite soundOnSprite, soundOffSprite; // Add images for sound off states
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject hidingPanel;
    [SerializeField] private AudioClip clickSound, toggleSound;

    [SerializeField] private GameObject newUserPanel;

    private AudioSource audioSource;

    private UserManager userManagerInstance;
    private AudioController audioController; // Reference to AudioController

    // Global variables to store music and sound settings
    private bool isMusicOn;
    private bool isSoundOn;
    public TMP_InputField tmp_InputField;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioController = FindObjectOfType<AudioController>(); // Find the AudioController in the scene

        closeButton.onClick.AddListener(CloseSettingsPanel);
        settingsButton.onClick.AddListener(OpenSettingsPanel);
        okayButton.onClick.AddListener(CloseSettingsPanel);
        musicButton.onClick.AddListener(ToggleMusic);
        soundButton.onClick.AddListener(ToggleSound);
        playButton.onClick.AddListener(OnPlayButtonClicked);
        LoadUsersData();

        // Default 1 for the sound and setting to PlayerPrefs
        if (!PlayerPrefs.HasKey(UserManager.SOUND_KEY) || !PlayerPrefs.HasKey(UserManager.MUSIC_KEY))
        {
            isSoundOn = true; // If the sound setting key is not found, set it to true (on) by default
            isMusicOn  = true;
            PlayerPrefs.SetInt(UserManager.SOUND_KEY, isSoundOn ? 1 : 0);
            PlayerPrefs.SetInt(UserManager.MUSIC_KEY, isMusicOn ? 1 : 0);
            PlayerPrefs.Save();
        }
        else 
        {
            LoadSoundSettings();
        }
    }

    private void OnPlayButtonClicked()
    {
        PlayClickSound(); // Play the click sound first
        StartCoroutine(LoadSceneWithDelay(1, clickSound.length - 0.3f)); // Load the scene after the sound finishes playing
    }

    private IEnumerator LoadSceneWithDelay(int sceneid, float delay)
    {
        yield return new WaitForSeconds(delay);
        NavigateToPlayScene(sceneid);
    }
    public void NavigateToPlayScene(int sceneid)
    {
        SceneManager.LoadScene(sceneid); // Load the scene after playing the sound
    }

    public void OpenSettingsPanel()
    {
        settingsPanel.SetActive(true);
        hidingPanel.SetActive(true);
        PlayClickSound();
    }

    public void CloseSettingsPanel()
    {
        settingsPanel.SetActive(false);
        hidingPanel.SetActive(false);
        UpdateUserInformation();
        PlayClickSound();
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
        PlayerPrefs.SetInt(UserManager.MUSIC_KEY, isMusicOn ? 1 : 0);
        PlayerPrefs.Save();

        PlayClickSound();
        // Update the music button image
        UpdateMusicButtonImage();
        audioController.ToggleBackgroundMusic();
    }

    private void ToggleSound()
    {
        audioSource.PlayOneShot(toggleSound);

        isSoundOn = !isSoundOn; // Toggle the sound state

        // Save the sound setting to PlayerPrefs
        Debug.Log(isSoundOn + "Sound Enabled");
        PlayerPrefs.SetInt(UserManager.SOUND_KEY, isSoundOn ? 1 : 0);
        PlayerPrefs.Save();

        PlayClickSound();

        // Update the sound button image
        UpdateSoundButtonImage();
    }

    private void UpdateMusicButtonImage()
    {
        musicButton.image.sprite = isMusicOn ? musicOnSprite : musicOffSprite;
    }

   private void UpdateSoundButtonImage()
    {
        soundButton.image.sprite = isSoundOn ? soundOnSprite : soundOffSprite;
    }

    private void LoadSoundSettings()
    {
        isMusicOn = PlayerPrefs.GetInt(UserManager.MUSIC_KEY, 1) == 1; // Default is true
        isSoundOn = PlayerPrefs.GetInt(UserManager.SOUND_KEY, 1) == 1; // Default is true
         // Toggle background music
        if(isMusicOn == false && PlayerPrefs.GetInt(UserManager.MUSIC_KEY, 1) == 0)
        {
            audioController.ToggleBackgroundMusic();
        }
        UpdateMusicButtonImage();
        UpdateSoundButtonImage();
    }

    private void UpdateUserInformation(){
        string name = tmp_InputField.text;
        userManagerInstance = FindObjectOfType<UserManager>();
        int image = userManagerInstance.GetImage();
        // Updating image here, as he can select image, but could wish to opt out of it
        PlayerPrefs.SetInt(UserManager.IMAGE_KEY, image);
        PlayerPrefs.Save();
        isMusicOn = PlayerPrefs.GetInt(UserManager.MUSIC_KEY, 1) == 1;
        isSoundOn = PlayerPrefs.GetInt(UserManager.SOUND_KEY, 1) == 1;
        string userID = PlayerPrefs.GetString(UserManager.USERID_KEY);
        StartCoroutine(UpdateUserDetails(userID, name, image.ToString(), isMusicOn.ToString(), isSoundOn.ToString()));
    }


    private IEnumerator UpdateUserDetails(string userID, string name, string profilePicture, string isMusicEnabled, string isSoundEnabled)
    {
        Debug.Log("[Home Scene]Updating User Details for " + userID);
        string deviceId = SystemInfo.deviceUniqueIdentifier;
        yield return StartCoroutine(UserProfile.UpdateUserDetails(userID, name, profilePicture, isMusicEnabled, isSoundEnabled,
            // onSuccess callback
            (userId) =>
            {
                

            },
            // onError callback
            (errorMessage) =>
            {
                Debug.LogError(errorMessage);
                // Handle error
            }
        ));
    }

    private void LoadUsersData()
    {
        // Loading the list of users to get the userID, name , sound settings
        userManagerInstance = FindObjectOfType<UserManager>();
        if (userManagerInstance != null)
        {
            // Call the GetUsers method
            User[] users = userManagerInstance.GetUsers();

            Debug.Log("[Home Scene]  Users Length" + users.Length);

            if (users == null || users.Length <= 0)
            {
                // No user found, to need to enforce new user addition
                Debug.Log("[Home Scene] No users found, so redirecting to user creation panel");
                newUserPanel.SetActive(true);
            } else {
                Debug.Log(UserManager.NAME_KEY);
                Debug.Log(PlayerPrefs.GetString(UserManager.NAME_KEY));
                tmp_InputField.text = PlayerPrefs.GetString(UserManager.NAME_KEY);
            }
        }
    }

}
