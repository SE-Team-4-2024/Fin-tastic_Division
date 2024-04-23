using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using GoogleMobileAds;
using GoogleMobileAds.Api;

public class HomeScene : MonoBehaviour
{
    [SerializeField] private Button okayButton, settingsButton, closeButton, userProfilesListButton;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject hidingPanel;
    [SerializeField] private GameObject userProfilesListPanel;
    [SerializeField] private GameObject newUserCreationPanel;
    [SerializeField] private AudioClip clickSound; // Add this field for the click sound

    private AudioSource audioSource; // Reference to AudioSource component
     public static User[] globalUsers;

    private void Start()
    {
        // Get the AudioSource component attached to this GameObject or add one if not present
        audioSource = GetComponent<AudioSource>();
        closeButton.onClick.AddListener(CloseSettingsPanel);
        settingsButton.onClick.AddListener(OpenSettingsPanel);
        okayButton.onClick.AddListener(CloseSettingsPanel);
        userProfilesListButton.onClick.AddListener(OpenProfilesListPanel);
        MobileAds.Initialize(initStatus => { });
        LoadUsersData();
    }

    public void OpenProfilesListPanel(){
        userProfilesListPanel.SetActive(true);
        DynamicUserProfileRenderer dynamicUserProfileRenderer = FindObjectOfType<DynamicUserProfileRenderer>();

        if (dynamicUserProfileRenderer != null)
        {
            // Call the LoadUsersData function from DynamicUserProfileRenderer
            dynamicUserProfileRenderer.LoadUsersData();
        }
        else
        {
            Debug.LogError("DynamicUserProfileRenderer not found in the scene.");
        }
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

    public void OnOkayButtonClick()
    {
        // Add any functionality you want for the okay button
        PlayClickSound();
    }

    private void PlayClickSound()
    {
        if (clickSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }

    // Update is called once per frame
    void Update()
    {
    }


    private void LoadUsersData()
    {
        UserManager userManagerInstance = FindObjectOfType<UserManager>();
        if (userManagerInstance != null)
        {
            // Call the GetUsers method
            User[] users = userManagerInstance.GetUsers();

            if (users != null && users.Length >= 1)
            {
                // More than one user
                Debug.Log("[Home Scene] Multiple users found. Handling multiple users case...");
                globalUsers = users;
            }
            else
            {
                // No users found, so need to create new user....
                Debug.Log("[Home Scene] No users found.");
                newUserCreationPanel.SetActive(true);
            }
        }
    }
}
