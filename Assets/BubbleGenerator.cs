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
        int RandomIndex = UnityEngine.Random.Range(0, 2);
        GameObject bubble = Instantiate(bubbles[RandomIndex]);

        float startY = UnityEngine.Random.Range(startPos.y - 1f, startPos.y + 1f);

        bubble.transform.position = new Vector3(startPos.x, startY, startPos.z);

        float scale = UnityEngine.Random.Range(0.8f, 1.2f);
        bubble.transform.localScale = new Vector2(scale, scale);
        float speed = UnityEngine.Random.Range(0.5f, 1.5f);
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
