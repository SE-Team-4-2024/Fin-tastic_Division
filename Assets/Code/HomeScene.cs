using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HomeScene : MonoBehaviour
{
    [SerializeField] private Button okayButton, settingsButton, closeButton;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject hidingPanel;
    [SerializeField] private AudioClip clickSound; // Add this field for the click sound

    private AudioSource audioSource; // Reference to AudioSource component

    private void Start()
    {
        // Get the AudioSource component attached to this GameObject or add one if not present
        audioSource = GetComponent<AudioSource>();
        
        closeButton.onClick.AddListener(CloseSettingsPanel);
        settingsButton.onClick.AddListener(OpenSettingsPanel);
        okayButton.onClick.AddListener(CloseSettingsPanel);
    }

    public void OpenSettingsPanel() 
    {
        settingsPanel.SetActive(true);
        hidingPanel.SetActive(true);
        PlayClickSound();
    }

    public void CloseSettingsPanel() 
    {
        settingsPanel.SetActive(false);
        hidingPanel.SetActive(false);
        PlayClickSound();
    }

    public void OnOkayButtonClick()
    {
        // Add any functionality you want for the okay button
        PlayClickSound();
    }

    private void PlayClickSound()
    {
        if (clickSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
