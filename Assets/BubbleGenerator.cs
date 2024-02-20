using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.Serialization;

public class BubbleGenerator : MonoBehaviour
{
    [SerializeField]
    GameObject[] bubbles;
    [SerializeField]
    float spawnInterval;
    [SerializeField]
    GameObject endPoint;
    Vector3 startPos;
    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
        Invoke("AttemptSpawn", spawnInterval);

    }

    // Update is called once per frame
    void SpawnBubble()
    {
        int RandomIndex = UnityEngine.Random.Range(0, bubbles.Length);
        GameObject bubble = Instantiate(bubbles[RandomIndex]);

        // Convert the screen's corner points to world coordinates
        Vector3 lowerLeft = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0));
        Vector3 upperRight = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));

        // Randomly pick a point within the screen
        float startX = UnityEngine.Random.Range(lowerLeft.x, upperRight.x);

        // Set startY to the bottom of the screen
        float startY = lowerLeft.y;

        bubble.transform.position = new Vector3(startX, startY, 0);

        float scale = UnityEngine.Random.Range(0.1f, 0.2f);
        bubble.transform.localScale = new Vector2(scale, scale);
        float speed = UnityEngine.Random.Range(0.9f, 1.8f);
        bubble.GetComponent<BubbleScript>().startFloating(speed, endPoint.transform.position.x);
    }


    void AttemptSpawn()
    {
        SpawnBubble();
        Invoke("AttemptSpawn", spawnInterval);

    }

    void Prewarm()
    {
        for (int i = 0; i < 10; i++)
        {
            Vector3 spawnPos = startPos + Vector3.right * (i * 2);
            //SpawnCloud(spawnPos);
        }
    }
}
