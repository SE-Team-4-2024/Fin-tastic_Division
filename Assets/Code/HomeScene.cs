using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HomeScene : MonoBehaviour
{
    public Button okayButton;


    // Start is called before the first frame update
    void Start()
    {
        okayButton.onClick.AddListener(CloseSettingsPanel);
    }
    public void CloseSettingsPanel() 
    {
        SceneManager.LoadScene("HomeViewController");

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
