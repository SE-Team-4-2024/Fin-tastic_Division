using UnityEngine;
using UnityEngine.UI;

public class DynamicUserProfileRenderer : MonoBehaviour
{
    public GameObject childPanelPrefab; // Reference to the prefab for the child panel
    public int numberOfPanels; // Number of child panels to create

    void Start()
    {
        // Loop to instantiate child panels
        for (int i = 0; i < numberOfPanels; i++)
        {
            // Instantiate the child panel prefab
            GameObject newPanel = Instantiate(childPanelPrefab, transform);

            // You can customize the newPanel here if needed
            // For example, set text or images on the panel based on data
        }
    }
}