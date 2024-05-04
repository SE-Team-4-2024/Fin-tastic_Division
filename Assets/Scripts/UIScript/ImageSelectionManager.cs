

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class ImageSelectionManager : MonoBehaviour
{
    [SerializeField] private GameObject profilePicturesPanel;

    [SerializeField] private GameObject newUserPanel;

    [SerializeField] private GameObject settingsPanel;

    [SerializeField] private Button okayButton;

    private Image imageComponent;


    void Start()
    {
        okayButton.onClick.AddListener(SetProfilePicture);
    }

    public void SetProfilePicture()
    {
        profilePicturesPanel.SetActive(false);
        if (isNewUserProfileCreation())
        {
            newUserPanel.SetActive(true);
            UpdateProfilePictureInNewUserPanel();
        }
        else
        {
            settingsPanel.SetActive(true);
            UpdateProfilePictureInSettingsPanel();
        }
    }

    private void UpdateProfilePictureInNewUserPanel()
    {

        GameObject otherGameObject = GameObject.Find("NewUserPanel");
        Transform imageTransform = otherGameObject.transform.Find("Profile");
        Image imageComponent = imageTransform.GetComponent<Image>();


        // Check if the Image component exists
        if (imageComponent == null)
        {
            Debug.LogError("Image component not found on the GameObject!");
            return;
        }

        UserManager userManagerInstance = FindObjectOfType<UserManager>();
        int image = userManagerInstance.GetImage();
        string imageName = userManagerInstance.GetImageName(image);

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
    }

    private void UpdateProfilePictureInSettingsPanel()
    {

        GameObject otherGameObject = GameObject.Find("SettingsPanel");
        Transform imageTransform = otherGameObject.transform.Find("Profile");
        Image imageComponent = imageTransform.GetComponent<Image>();


        // Check if the Image component exists
        if (imageComponent == null)
        {
            Debug.LogError("Image component not found on the GameObject!");
            return;
        }

        UserManager userManagerInstance = FindObjectOfType<UserManager>();
        int image = userManagerInstance.GetImage();
        string imageName = userManagerInstance.GetImageName(image);

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
    }

    public bool isNewUserProfileCreation()
    {
        UserManager userManagerInstance = FindObjectOfType<UserManager>();
        if (userManagerInstance != null)
        {
            // Call the GetUsers method
            User[] users = userManagerInstance.GetUsers();

            return users.Length == 0;
        }

        return true;
    }

}