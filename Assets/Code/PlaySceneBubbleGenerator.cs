using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.Serialization;

public class PlaySceneBubbleGenerator : MonoBehaviour
{
    [SerializeField]
    GameObject[] bubbles;
    [SerializeField]
    float spawnInterval;
    [SerializeField]
    GameObject endPoint;
    Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
        Invoke("AttemptSpawn", spawnInterval);
    }

    void SpawnBubble()
    {
        int randomIndex = Random.Range(0, bubbles.Length);
        GameObject bubble = Instantiate(bubbles[randomIndex]);

        // Get the left and right edges of the screen
        float leftEdge = Camera.main.ScreenToWorldPoint(Vector3.zero).x;
        float rightEdge = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x;

        // Set startX to a random position between the left and right edges
        float startX = transform.position.x;

        // Set startY to the bottom of the screen
        float startY = Random.Range(transform.position.y-1, transform.position.y+1);

        bubble.transform.position = new Vector3(startX, startY, 0);

        float scale = UnityEngine.Random.Range(0.01f, 0.05f);
        bubble.transform.localScale = new Vector2(scale, scale);
        float speed = UnityEngine.Random.Range(0.9f, 1f);
        bubble.GetComponent<PlaySceneBubbleScript>().StartFloating(speed, endPoint.transform.position.x);
    }

    void AttemptSpawn()
    {
        SpawnBubble();
        Invoke("AttemptSpawn", spawnInterval);
    }
}
