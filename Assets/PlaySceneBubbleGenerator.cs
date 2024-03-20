using UnityEngine;

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
        Debug.Log("Attempting to spawn bubble.");
        int randomIndex = Random.Range(0, bubbles.Length);
        GameObject bubble = Instantiate(bubbles[randomIndex]);

        // Get the left and right edges of the screen
        float leftEdge = Camera.main.ScreenToWorldPoint(Vector3.zero).x;
        float rightEdge = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x;

        // Set startX to a random position between the left and right edges
        float startX = Random.Range(leftEdge, rightEdge);

        // Set startY to the bottom of the screen
        float startY = Camera.main.ScreenToWorldPoint(Vector3.zero).y;

        bubble.transform.position = new Vector3(startX, startY, 0);

        float scale = Random.Range(0.1f, 0.2f);
        bubble.transform.localScale = new Vector2(scale, scale);
        float speed = Random.Range(0.9f, 1.8f);
        bubble.GetComponent<PlaySceneBubbleScript>().StartFloating(speed, endPoint.transform.position.x);
        Debug.Log("Bubble spawned at position: " + bubble.transform.position);
    }

    void AttemptSpawn()
    {
        SpawnBubble();
        Invoke("AttemptSpawn", spawnInterval);
    }
}
