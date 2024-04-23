using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class DynamicUserProfileRenderer : MonoBehaviour
{
    public GameObject panelPrefab; // Prefab for the panel
    public RectTransform content; // Content transform of the ScrollView
    public float spacing = 30f; // Spacing between panels
    public int panelsPerRow = 1; // Number of panels per row



    void Start()
    {
        LoadUsersData();
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
                PopulateScrollView(users);
            }
        }
    }

    void PopulateScrollView(User[] users)
    {
        int numberOfItems = users.Length; // Use the length of the names array
        float itemSpacing = 10f; // Adjust this value to set the spacing between items
        panelPrefab.SetActive(false);

        int rowCount = Mathf.CeilToInt((float)numberOfItems / panelsPerRow); // Calculate number of rows

        // Calculate the height of each panel
        float panelHeight = panelPrefab.GetComponent<RectTransform>().rect.height;

        // Calculate the total height of all panels with extra space at the top and bottom
        float totalPanelHeight = rowCount * panelHeight + (rowCount - 1) * spacing;

        // Add extra space to the top and bottom
        float topBottomSpace = 200f; // Increase this value as needed
        totalPanelHeight += topBottomSpace * 2;

        // Update the size of the content RectTransform
        content.sizeDelta = new Vector2(content.sizeDelta.x, totalPanelHeight);

        for (int i = 0; i < rowCount; i++)
        {
            for (int j = 0; j < panelsPerRow; j++)
            {
                int index = i * panelsPerRow + j;
                Debug.Log("Index" + index);
                Debug.Log("numberOfItems" + numberOfItems);
                if (index >= numberOfItems) return; // Exit loop if all items are created

                GameObject panel = Instantiate(panelPrefab, content);
                panel.SetActive(true);
                RectTransform panelRectTransform = panel.GetComponent<RectTransform>();

                // Calculate position of the panel
                float xPos = j * (panelRectTransform.rect.width + spacing);
                float yPos = -i * (panelHeight + spacing);

                // Adjust position to add spacing between items
                xPos += j * itemSpacing;

                panelRectTransform.anchoredPosition = new Vector2(xPos, yPos);

                // Set the size of the panel
                panelRectTransform.sizeDelta = new Vector2(panelRectTransform.sizeDelta.x, panelHeight);
                panel.name = "Panel" + index.ToString();
                User user = users[index];

                // Set the name text for the panel using the provided names array
                SetPanelText(panel, user.Name);
                SetPanelUserData(panel, user);
            }
        }
    }

     private void SetPanelUserData(GameObject panel, User user){
        PanelUserData panelUserData = panel.GetComponentInChildren<PanelUserData>();
        if (panelUserData == null)
        {
            Debug.LogError("PanelUserData script not found on panel GameObject.");
            return;
        }
        panelUserData.SetCustomValues(user.Name, user.UserID);

        Debug.Log(panelUserData.GetUserID());
     }



    private void SetPanelText(GameObject panel, string name)
    {
        TextMeshProUGUI panelText = panel.GetComponentInChildren<TextMeshProUGUI>();
        if (panelText != null)
        {
            // Set the text you want to display
            panelText.text = name; // Change this to your desired text
            Debug.Log("Text set successfully for panel " + name);
        }
        else
        {
            Debug.LogError("Text component not found in panel " + name);
        }
    }
}

