using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMusicManager : MonoBehaviour
{
    // This static variable will hold a reference to the single instance of the BackgroundMusic object.
    private static BGMusicManager instance = null;

    public static BGMusicManager Instance
    {
        get { return instance; }
    }

    void Awake()
    {
        // If an instance already exists and it's not this one, destroy this one and return.
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            // This is the first instance - make it the Singleton.
            instance = this;
        }

        // Use this to keep the object alive across scenes.
        DontDestroyOnLoad(this.gameObject);
    }
}
