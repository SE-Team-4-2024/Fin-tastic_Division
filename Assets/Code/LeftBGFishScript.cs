using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftBGFishScript : MonoBehaviour
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
            transform.position += new Vector3(speed * Time.deltaTime, 0, 0);

            // Check if the bubble has passed the screen
            if (transform.position.x > endPointX)
            {
                // Destroy the bubble
                Destroy(gameObject);
            }
        }
    }
}
