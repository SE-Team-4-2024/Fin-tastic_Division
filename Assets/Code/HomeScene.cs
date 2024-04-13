using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HomeScene : MonoBehaviour
{
    [SerializeField]
    public Button okayButton, settingsButton;
    [SerializeField]
    private GameObject settingsPanel;
     
    [SerializeField]
    private GameObject backgroundOverlayPanel;
    public void OnButtonClick()
    {
        // Activate the settings panel
        settingsPanel.SetActive(true);
        
        backgroundOverlayPanel.SetActive(true);
    }

    // Start is called before the first frame update
    void Start()
    {
        //settingsButton.onClick.AddListener(OpenSettingsPanel);
        okayButton.onClick.AddListener(CloseSettingsPanel);
        
    }

    public void OpenSettingsPanel() 
    {
        settingsPanel.SetActive(true);
        backgroundOverlayPanel.SetActive(true);
    }
    public void CloseSettingsPanel() 
    {
        settingsPanel.SetActive(false);
        backgroundOverlayPanel.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
