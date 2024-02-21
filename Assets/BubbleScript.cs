using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleScript : MonoBehaviour
{
    private bool isFloating = false;
    private float speed;
    private float endPointX;

    public void startFloating(float speed, float endPointX)
    {
        this.speed = speed;
        this.endPointX = endPointX;
        this.isFloating = true;
    }

    void Update()
    {
        if (isFloating)
        {
            // Move the bubble up
            transform.position += new Vector3(0, speed * Time.deltaTime, 0);

            // Get the top of the screen
            float topOfScreen = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, 0)).y;

            // Check if the bubble has reached the top of the screen
            if (transform.position.y >= topOfScreen)
            {
                // Destroy the bubble
                Destroy(gameObject);
            }
        }
    }
}