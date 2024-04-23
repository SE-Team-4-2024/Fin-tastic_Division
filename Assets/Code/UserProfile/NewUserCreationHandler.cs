using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NewUserCreationHandler : MonoBehaviour
{
    public GameObject newUserAdditionPanel; // Panel on which the new user is created.
    [SerializeField] private Button okayButton;
    private InputField[] inputFields; // Array to store references to input fields

    private void Start()
    {
        inputFields = newUserAdditionPanel.GetComponentsInChildren<InputField>();
        okayButton.onClick.AddListener(OnCreateButtonClick);
    }

    public void OnCreateButtonClick()
    {
        string name = inputFields[0].text;
        StartCoroutine(GetorCreateUser(name));
        
    }


    private IEnumerator GetorCreateUser(string name)
    {
        string deviceId = SystemInfo.deviceUniqueIdentifier;
        string profilePicture = "1";
       Debug.Log("[New User Creation Handler] Creating New User For " + name + " " + deviceId);
        yield return StartCoroutine(UserProfile.GetorCreateUser(deviceId, name, profilePicture,
            // onSuccess callback
            (userId) =>
            {
                Debug.Log("[New User Creation Handler] User Id "+ userId);
                PlayerPrefs.SetString("userID", userId);
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