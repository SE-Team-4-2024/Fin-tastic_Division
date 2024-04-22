using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HomeScene : MonoBehaviour
{
    [SerializeField] private Button okayButton, settingsButton, closeButton, musicButton, soundButton, playButton;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject hidingPanel;
    [SerializeField] private AudioClip clickSound, toggleSound;

    private AudioSource audioSource;
    private AudioController audioController; // Reference to AudioController

    // Global variables to store music and sound settings
    private bool isMusicOn;
    private bool isSoundOn;
    private const string MUSIC_KEY = "MusicOn";
    private const string SOUND_KEY = "SoundOn";

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

        // Default 1 for the sound and setting to PlayerPrefs
        if (!PlayerPrefs.HasKey(SOUND_KEY) || !PlayerPrefs.HasKey(MUSIC_KEY))
        {
            isSoundOn = true; // If the sound setting key is not found, set it to true (on) by default
            isMusicOn  = true;
            PlayerPrefs.SetInt(SOUND_KEY, isSoundOn ? 1 : 0);
            PlayerPrefs.SetInt(MUSIC_KEY, isMusicOn ? 1 : 0);
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
        PlayerPrefs.SetInt(MUSIC_KEY, isMusicOn ? 1 : 0);
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
        PlayerPrefs.SetInt(SOUND_KEY, isSoundOn ? 1 : 0);
        PlayerPrefs.Save();

        PlayClickSound();
    }

    private void LoadSoundSettings()
    {
        isMusicOn = PlayerPrefs.GetInt(MUSIC_KEY, 1) == 1; // Default is true
        isSoundOn = PlayerPrefs.GetInt(SOUND_KEY, 1) == 1; // Default is true
    }

}
