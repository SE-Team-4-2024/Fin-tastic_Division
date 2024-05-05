using UnityEngine;
using System.Collections;

public class AudioController : MonoBehaviour
{
    public AudioSource backgroundMusic;
    public AudioClip panelSoundClip;

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
        backgroundMusic.volume = 0.3f;
        backgroundMusic.loop = true;  // Set background music to loop
    }

    public void ToggleBackgroundMusic(bool isMusicOn)
    {
        if (isMusicOn)
        {
            backgroundMusic.Play();
            Debug.Log("Background music is playing.");
        }
        else
        {
            backgroundMusic.Pause();
            Debug.Log("Background music is paused.");
        }
    }

    public void PlayPanelSoundAndResumeBackgroundMusic()
    {
        backgroundMusic.Pause();
        StartCoroutine(PlaySoundAndWait(panelSoundClip));
    }

    private IEnumerator PlaySoundAndWait(AudioClip clip)
    {
        backgroundMusic.PlayOneShot(clip);
        yield return new WaitForSeconds(clip.length);
        //backgroundMusic.Resume();  // Resume background music after panel sound
    }

    public void ResumeBackgroundMusic()
    {
        backgroundMusic.UnPause();
    }
}