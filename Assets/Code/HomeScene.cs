using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HomeScene : MonoBehaviour
{
    [SerializeField]
    public Button okayButton, settingsButton, closeButton;
    [SerializeField]
    private GameObject settingsPanel;
    [SerializeField]
    private GameObject hidingPanel;
    public void OnButtonClick()
    {
        // Activate the settings panel
        settingsPanel.SetActive(true);
        hidingPanel.SetActive(true);
    }

    // Start is called before the first frame update
    void Start()
    {
        closeButton.onClick.AddListener(CloseSettingsPanel);
        settingsButton.onClick.AddListener(OpenSettingsPanel);
        okayButton.onClick.AddListener(CloseSettingsPanel);
    }

    public void OpenSettingsPanel() 
    {
        settingsPanel.SetActive(true);
        hidingPanel.SetActive(true);
    }
    public void CloseSettingsPanel() 
    {
        settingsPanel.SetActive(false);
        hidingPanel.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
