using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    public AudioSource backgroundMusic;
    public AudioClip panelSoundClip; // Assign the sound clip directly

    public static AudioController instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayPanelSoundAndResumeBackgroundMusic()
    {
        backgroundMusic.Pause();
        StartCoroutine(PlaySoundAndWait(panelSoundClip));
    }

    private IEnumerator PlaySoundAndWait(AudioClip clip)
    {
        // Play the panel sound
        backgroundMusic.PlayOneShot(clip);

        // Wait for the clip to finish
        yield return new WaitForSeconds(clip.length);

        // Resume background music
        //backgroundMusic.UnPause();
    }
}
