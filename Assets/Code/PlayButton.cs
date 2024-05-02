using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayButton : MonoBehaviour
{
    [SerializeField] private AudioClip clickSound; // Add this field for the click sound
    private AudioSource audioSource; // Reference to AudioSource component

    private void Start()
    {
        // Get the AudioSource component attached to this GameObject or add one if not present
        audioSource = GetComponent<AudioSource>();
    }

    public void NavigateToPlayScene(int sceneid)
    {
        PlayClickSound(); // Play the click sound first
        SceneManager.LoadScene(sceneid); // Load the scene after playing the sound
    }

    private void PlayClickSound()
    {
        if (clickSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }
}


/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayButtonFunc : MonoBehaviour
{
    public Button StartButton;
    // Start is called before the first frame update
    void Start()
    {
        StartButton.onClick.AddListener(LoadDivisionScreen);
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void LoadDivisionScreen()
    {
        SceneManager.LoadScene("DivisionScreen");
    }
}*/

