using UnityEngine;

public class PlaySceneBubbleScript : MonoBehaviour
{
    private bool isFloating = false;
    private float speed;
    private float endPointX;

    public void StartFloating(float speed, float endPointX)
    {
        this.speed = speed;
        this.endPointX = endPointX;
        this.isFloating = true;
    }

    void Update()
    {
        if (isFloating)
        {
            // Move the bubble up and slightly to the left
            transform.position += new Vector3(-speed * Time.deltaTime, (float)speed/4 * Time.deltaTime, 0);

            // Get the right edge of the screen
            float rightEdge = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, 0)).x;

            // Check if the bubble has passed the screen
            if (transform.position.x < rightEdge)
            {
                // Destroy the bubble
                Destroy(gameObject);
            }
        }
    }
}
