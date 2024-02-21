using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsOnClick : MonoBehaviour
{
    [SerializeField]
    private GameObject settingsPanel;
    // Start is called before the first frame update
    [SerializeField]
    public void OnButtonClick()
    {
        // Activate the settings panel
        settingsPanel.SetActive(true);
        Debug.Log("Button Clicked!");
        // Example: Change the text of the button when clicked
    }
}
