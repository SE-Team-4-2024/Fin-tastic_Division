using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftBGFishGenerator : MonoBehaviour
{
    [SerializeField]
    GameObject[] bgFishs;
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

    void SpawnCoral()
    {
        int randomIndex = Random.Range(0, bgFishs.Length);
        GameObject bgFish = Instantiate(bgFishs[randomIndex]);

        // Get the left and right edges of the screen
        float leftEdge = Camera.main.ScreenToWorldPoint(Vector3.zero).x;
        float rightEdge = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x;

        // Set startX to a random position between the left and right edges
        float startX = transform.position.x;

        // Set startY to the bottom of the screen
        float startY = Random.Range(transform.position.y-1, transform.position.y+1);

        bgFish.transform.position = new Vector3(startX, startY, 0);

        float scale = UnityEngine.Random.Range(0.1f, 0.5f);
        bgFish.transform.localScale = new Vector2(scale, scale);
        float speed = 0.55f;
        bgFish.GetComponent<LeftBGFishScript>().StartFloating(speed, endPoint.transform.position.x);
    }

    void AttemptSpawn()
    {
        SpawnCoral();
        Invoke("AttemptSpawn", spawnInterval);
    }
}
