using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGBehavior : MonoBehaviour
{
    public float scrollSpeed;
    private float spriteWidth;
    private Vector3 startPosition;

    void Start()
    {
        // Save the starting position of the sprite.
        startPosition = transform.position;
        // Automatically calculate the width of the sprite.
        spriteWidth = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void Update()
    {
        // Move the background
        float newPosition = Mathf.Repeat(Time.time * scrollSpeed, spriteWidth);
        transform.position = startPosition + Vector3.left * newPosition;

        // Check if the sprite has moved beyond its width and reset its position
        if (newPosition >= spriteWidth)
        {
            transform.position = startPosition;
        }
    }
}