using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;// Add this line to include the UnityEngine.UI namespace

public class EditButton : MonoBehaviour
{
    // Declare serialized fields for the buttons
    [SerializeField] private Button editUserButton;
    [SerializeField] private GameObject EditUserPanel;
    [SerializeField] private Button addUserButton;
    [SerializeField] private GameObject UserProfilesPanel;

    [SerializeField] private GameObject NewUserCreationPanel;

    public InputField inputField;

    // Start is called before the first frame update
    void Start()
    {
        editUserButton.onClick.AddListener(OpenEditUserPanel);
        addUserButton.onClick.AddListener(OpenAddUserPanel);
        // Attach the OpenNewUserPanel method to the onClick event of the editButton
    }


    // Method to be called when editButton is clicked
    public void OpenEditUserPanel()
    {
        Debug.Log("[Edit User Profile Button]");

        GameObject parentPrefab = transform.parent.gameObject;

        UserManager userManagerInstance = FindObjectOfType<UserManager>();
        PanelUserData panelUserData = parentPrefab.GetComponentInChildren<PanelUserData>();

        if (panelUserData != null)
        {
            Debug.Log(panelUserData.name + "Name Set");
        }

        if (userManagerInstance != null)
        {
            EditUserPanel.SetActive(true);
            UserProfilesPanel.SetActive(false);
            UpdateInputFieldValue(panelUserData.name);
            userManagerInstance.SetEditUserID(panelUserData.userID);
        }

    }

    public void UpdateInputFieldValue(string name)
    {
        Debug.Log("[Edit User Profile] Trying to update name" + name);

        // Get the InputField component if it's not already assigned
        if (inputField == null)
        {
            inputField = GetComponent<InputField>();
        }

        // Attempt to get the Text component of the InputField
        Text textComponent = inputField.GetComponentInChildren<Text>();

        // Check if the Text component is found
        if (textComponent != null)
        {
            //Updating the text Value
            textComponent.text = name ?? "Enter Your Name";
            Debug.Log("Updated text value: " + textComponent.text);
        }
    }

    public void OpenAddUserPanel(){
        NewUserCreationPanel.SetActive(true);
        UserProfilesPanel.SetActive(false);
    }
}
