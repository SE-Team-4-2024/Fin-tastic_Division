using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class HomeScene : MonoBehaviour
{
    [SerializeField] private Button okayButton, settingsButton, closeButton, musicButton, soundButton, playButton;// historyButton, closeButtonPrev;
    [SerializeField] private Sprite musicOnSprite, musicOffSprite; // Add images for music off states
    [SerializeField] private Sprite soundOnSprite, soundOffSprite; // Add images for sound off states
    [SerializeField] private GameObject settingsPanel; //previousRecordsPanel;
    [SerializeField] private GameObject hidingPanel;
    [SerializeField] private AudioClip clickSound, toggleSound;

    [SerializeField] private GameObject newUserPanel;
    private Game[] fetchedGameStats;
    public GameObject textBoxPrefab; // Reference to the prefab of your text box


    private Game[] fetchedGames; 

    private List<GameObject> createdTextBoxes = new List<GameObject>(); // List to store references to created text boxes


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
        // historyButton.onClick.AddListener(() => { ShowPreviousRecords(); PlayClickSound(); });
        // closeButtonPrev.onClick.AddListener(() => {ClosePreviousrecordsPanel(); PlayClickSound(); });


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
            Debug.Log("LoadSoundSettings");
        }
    }

    private void OnPlayButtonClicked()
    {
        PlayClickSound();
        StartCoroutine(LoadSceneWithDelay(1, clickSound.length - 0.3f)); // Load the scene after the sound finishes playing
        playButton.gameObject.SetActive(false); 
    }

    public void EnablePlayButton()
    {
        playButton.gameObject.SetActive(true); 
        playButton.interactable = true;
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
        audioController.ToggleBackgroundMusic(isMusicOn);
    }

    private void ToggleSound()
    {
        audioSource.PlayOneShot(toggleSound);

        isSoundOn = !isSoundOn; // Toggle the sound state

        // Save the sound setting to PlayerPrefs
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
        Debug.Log("music" + isMusicOn);
        Debug.Log("sound" + isSoundOn);
        audioController.ToggleBackgroundMusic(isMusicOn);
        UpdateMusicButtonImage();
        UpdateSoundButtonImage();
    }

    // void ShowPreviousRecords()
    // {
    //     LoadGameStats();
    //     hidingPanel.SetActive(true);
    //     previousRecordsPanel.SetActive(true);
    //     CreateTextBoxes(5);
    // }

    void OnEnable()
    {
        Debug.Log("Scene initialized or re-initialized....");
        //LoadGameStats();
    }
    

    // void ClosePreviousrecordsPanel(){
    //     DestroyTextBoxes();
    //     previousRecordsPanel.SetActive(false);
    //     hidingPanel.SetActive(false);
    // }

    // void CreateTextBoxes(int limit)
    // {
    //     textBoxPrefab.SetActive(true);
    //     float verticalSpacing = 10f;
    //     // Get the position of the prefab text box
    //     Vector3 prefabPosition = textBoxPrefab.transform.position;

    //     // Get the height of the prefab text box
    //     float textBoxHeight = textBoxPrefab.GetComponent<TextMeshProUGUI>().rectTransform.rect.height;
    //     // Get the font size of the prefab text box
    //     float fontSize = textBoxPrefab.GetComponent<TextMeshProUGUI>().fontSize;
    //     // Loop through the number of rows
    //     for (int i = 0; i < fetchedGameStats.Length && i < limit; i++)
    //     {
    //         // Instantiate a new text box prefab
    //         GameObject newTextBox = Instantiate(textBoxPrefab, transform);
    //         createdTextBoxes.Add(newTextBox); 
    //         // Set font size for the new text box
    //         TextMeshProUGUI textComponent = newTextBox.GetComponent<TextMeshProUGUI>();
    //         textComponent.fontSize = fontSize;
    //         textComponent.text = (i + 1).ToString() + ". Score: " + fetchedGameStats[i].noOfCorrectAnswers.ToString() + "/" +  "5";

    //         // Calculate position for the new text box
    //         float newY = prefabPosition.y - ((i + 1) * (textBoxHeight + verticalSpacing)); // Adding 1 to i because we want the new boxes to be below the prefab
    //         Vector3 newPosition = new Vector3(prefabPosition.x, newY, prefabPosition.z);

    //         // Set position for the new text box
    //         newTextBox.transform.position = newPosition;

    //         // You might want to modify other properties of the text box (like text content) here
    //     }
    //     // Deactivate the initial prefab
    //     textBoxPrefab.SetActive(false);
    // }

    //  void DestroyTextBoxes()
    // {   
    //     foreach (var textBox in createdTextBoxes)
    //     {
    //         Destroy(textBox);
    //     }
    //     createdTextBoxes.Clear();
    // }

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
        // string deviceId = SystemInfo.deviceUniqueIdentifier;
        string deviceId = "5D16B0D8-91B7-52E1-9A0B-2A5F635A0A35";
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
        Debug.Log("Loadbng Users");
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
                //LoadGameStats();
            }
        }
    }

    // private void LoadGameStats()
    // {
    //     userManagerInstance = FindObjectOfType<UserManager>();
    //     fetchedGameStats = userManagerInstance.FetchGameStatsData();
    //     Debug.Log(fetchedGameStats.Length + "Length");
    //     if (fetchedGameStats.Length < 1)
    //     {
    //         previousRecordsPanel.SetActive(false);
    //         historyButton.gameObject.SetActive(false);
    //     } else {
    //         historyButton.gameObject.SetActive(true);
    //     }
    // }

}
