using System.Collections;
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
}
