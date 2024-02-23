using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayButton : MonoBehaviour
{
    [SerializeField]
    // Start is called before the first frame update
    public void NavigateToPlayScene(int sceneid)
    {
        SceneManager.LoadScene(sceneid);
        // Example: Change the text of the button when clicked
    }
}
