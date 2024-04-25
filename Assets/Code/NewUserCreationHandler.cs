using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class NewUserCreationHandler : MonoBehaviour
{
    public GameObject newUserAdditionPanel; // Panel on which the new user is created.

    [SerializeField] private Sprite musicOnSprite, musicOffSprite; // Add images for music off states
    [SerializeField] private Sprite soundOnSprite, soundOffSprite; // Add images for sound off states
    [SerializeField] private Button createButton;
    private InputField[] inputFields; // Array to store references to input fields
   [SerializeField] private Button musicButton, soundButton;
    [SerializeField] private AudioClip clickSound, toggleSound;

    [SerializeField] private GameObject settingsPanel;

    public TMP_InputField tmp_InputField;

    private AudioSource audioSource;
    private AudioController audioController; // Reference to AudioController

    // Global variables to store music and sound settings
    private bool isMusicOn;
    private bool isSoundOn;


    private void Start()
    {

        // Find the AudioController in the scene
        audioSource = GetComponent<AudioSource>();
        audioController = FindObjectOfType<AudioController>(); 
        inputFields = newUserAdditionPanel.GetComponentsInChildren<InputField>();
        musicButton.onClick.AddListener(ToggleMusic);
        soundButton.onClick.AddListener(ToggleSound);
        createButton.onClick.AddListener(OnCreateButtonClick);

        isSoundOn = true; // If the sound setting key is not found, set it to true (on) by default
        isMusicOn = true;
        PlayerPrefs.SetInt(UserManager.SOUND_KEY, isSoundOn ? 1 : 0);
        PlayerPrefs.SetInt(UserManager.MUSIC_KEY, isMusicOn ? 1 : 0);
        PlayerPrefs.Save();

        UpdateMusicButtonImage();
        UpdateSoundButtonImage();
    }

    public void OnCreateButtonClick()
    {
        string name = inputFields[0].text;
        UserManager userManagerInstance = FindObjectOfType<UserManager>();
        int profilePicture =  userManagerInstance.GetImage();
        bool isMusicEnabled = PlayerPrefs.GetInt(UserManager.MUSIC_KEY, 1) == 1;
        bool isSoundEnabled = PlayerPrefs.GetInt(UserManager.SOUND_KEY, 1) == 1;
        StartCoroutine(GetorCreateUser(name, profilePicture, isMusicEnabled, isSoundEnabled));
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

    private void UpdateMusicButtonImage()
    {
        musicButton.image.sprite = isMusicOn ? musicOnSprite : musicOffSprite;
    }

   private void UpdateSoundButtonImage()
    {
        soundButton.image.sprite = isSoundOn ? soundOnSprite : soundOffSprite;
    }

    private void ToggleMusic()
    {
        audioSource.PlayOneShot(toggleSound);
        isMusicOn = !isMusicOn; // Toggle the music state

        // Save the music setting to PlayerPrefs
        PlayerPrefs.SetInt(UserManager.MUSIC_KEY, isMusicOn ? 1 : 0);
        PlayerPrefs.Save();

        PlayClickSound();

        // Toggle background music
        audioController.ToggleBackgroundMusic();
        UpdateMusicButtonImage();

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
        UpdateSoundButtonImage();
    }

    private void UploadDetailsForSettingsPanel(string name, int profilePicture, bool isMusicEnabled, bool isSoundEnabled){
        settingsPanel.SetActive(true);
        tmp_InputField.text = name; // Loading the name for settings scene
        GameObject otherGameObject = GameObject.FindWithTag("SettingsPanel");
        Transform imageTransform = otherGameObject.transform.Find("Profile");

        Image imageComponent = imageTransform.GetComponent<Image>();


        // Check if the Image component exists
        if (imageComponent == null)
        {
            Debug.LogError("Image component not found on the GameObject!");
            return;
        }
         Debug.Log("image Found");

        UserManager userManagerInstance = FindObjectOfType<UserManager>();
         Debug.Log("instance panel");
        string imageName = userManagerInstance.GetImageName(profilePicture);

        // Load the sprite from the Resources folder
        Sprite newSprite = Resources.Load<Sprite>(imageName);

        // Check if the sprite was successfully loaded
        if (newSprite != null)
        {
            // Update the sprite of the Image component
            imageComponent.sprite = newSprite;
        }
        else
        {
            Debug.LogError("Sprite not found at path: " + imageName);
        }

        isMusicOn = isMusicEnabled;
        isSoundOn = isSoundEnabled;
        UpdateSoundButtonImage();
        UpdateMusicButtonImage();
    }


    private IEnumerator GetorCreateUser(string name, int profilePicture, bool isMusicEnabled, bool isSoundEnabled)
    {
        string deviceId = SystemInfo.deviceUniqueIdentifier;
        Debug.Log("[New User Creation Handler] Creating New User For " + name + " " + deviceId);
        yield return StartCoroutine(UserProfile.GetorCreateUser(deviceId, name, profilePicture.ToString(), isMusicEnabled.ToString(), isSoundEnabled.ToString(),
            // onSuccess callback
            (userId) =>
            {   // Saving the name and user id , if api is successful.
                PlayerPrefs.SetString(UserManager.USERID_KEY, userId);
                PlayerPrefs.SetString(UserManager.NAME_KEY, name);
                PlayerPrefs.SetInt(UserManager.SOUND_KEY, isSoundEnabled ? 1 : 0);
                PlayerPrefs.SetInt(UserManager.MUSIC_KEY, isMusicEnabled ? 1 : 0);
                UploadDetailsForSettingsPanel(name, profilePicture, isMusicEnabled, isSoundEnabled);
                Debug.Log("[New User Creation Handler] User Id "+ userId);
                newUserAdditionPanel.SetActive(false); // Once passes, the panel is not needed
                settingsPanel.SetActive(false);
            },
            // onError callback
            (errorMessage) =>
            {
                Debug.LogError("[New User Creation Handler] Failed to create error " + errorMessage);
            }
        ));
    }
}