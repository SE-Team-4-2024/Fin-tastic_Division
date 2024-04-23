using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UpdateUser : MonoBehaviour
{
    public GameObject editUserPanel; // Reference to the panel containing the input fields
    [SerializeField] private Button okayButton;
    private InputField[] inputFields; // Array to store references to input fields

    private void Start()
    {
        // Get references to all input fields in the panel
        inputFields = editUserPanel.GetComponentsInChildren<InputField>();
        okayButton.onClick.AddListener(OnButtonClick);
    }

    public void OnButtonClick()
    {
        // Iterate over each input field and retrieve its value
        Debug.Log("[Update User] Updating the user details");
        string name = inputFields[0].text;
        UserManager userManagerInstance = FindObjectOfType<UserManager>();
        string userID = userManagerInstance.GetEditUserID();
        StartCoroutine(UpdateUserDetails(userID, name, "1", true.ToString(), false.ToString()));
        editUserPanel.SetActive(false);
        userManagerInstance.GetUsers();
        Debug.Log("after udate");
    }


    private IEnumerator UpdateUserDetails(string userID, string name, string profilePicture, string isMusicEnabled, string isSoundEnabled)
    {

        Debug.Log("[Update User] Updating User Details for " + userID);

        string deviceId = SystemInfo.deviceUniqueIdentifier;

        yield return StartCoroutine(UserProfile.UpdateUserDetails(userID, name, profilePicture, isMusicEnabled, isSoundEnabled,
            // onSuccess callback
            (userId) =>
            {
                Debug.Log("[Details Updated]....");
                Debug.Log(userId);

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


}