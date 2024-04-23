using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpdatePrimaryUser : MonoBehaviour
{
    [SerializeField] private Button makePrimary;
    [SerializeField] private GameObject userProfilesListPanel;

    private void Start()
    {
        makePrimary.onClick.AddListener(UpdatePrimaryUsers);
    }

    public void UpdatePrimaryUsers()
    {
        GameObject parentPrefab = transform.parent.gameObject;

        UserManager userManagerInstance = FindObjectOfType<UserManager>();
        PanelUserData panelUserData = parentPrefab.GetComponentInChildren<PanelUserData>();

        if (panelUserData != null)
        {
            Debug.Log(panelUserData.name + " Name Set");
        }

        if (userManagerInstance != null)
        {
            Debug.Log("Inside..");
            StartCoroutine(UpdatePrimaryUsersCoroutine(panelUserData.userID)); // Start the coroutine
            userManagerInstance.SetEditUserID(panelUserData.userID);
        }
    }

    private IEnumerator UpdatePrimaryUsersCoroutine(string userID)
    {
        PlayerPrefs.SetString("userID", userID);
        string deviceId = SystemInfo.deviceUniqueIdentifier;

        yield return StartCoroutine(UserProfile.UpdatePrimaryUser(userID, deviceId,
            // onSuccess callback
            (response) =>
            {
                Debug.Log("Profile User Updated Successfully" + response);
                // Handle successful creation
            },
            // onError callback
            (errorMessage) =>
            {
                Debug.LogError(errorMessage);
                // Handle error
            }
        ));

        userProfilesListPanel.SetActive(false); // Reactivate the panel after coroutine completes
    }
}
