using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGBehavior : MonoBehaviour
{
    public float scrollSpeed;
    private float spriteWidth;
    private Vector3 startPosition;
    private float movedDistance;

    void Start()
    {
        // Save the starting position of the sprite.
        startPosition = transform.position;
        // Automatically calculate the width of the sprite.
        spriteWidth = GetComponent<SpriteRenderer>().bounds.size.x;
        movedDistance = 0f;
    }

    void Update()
    {
        // Move the background
        movedDistance += Time.deltaTime * scrollSpeed;
        movedDistance = Mathf.Repeat(movedDistance, spriteWidth);

        transform.position = startPosition + Vector3.left * movedDistance;

        // Check if the sprite has moved beyond its width and reset its position
        if (movedDistance >= spriteWidth/1.5)
        {
            transform.position = startPosition;
            movedDistance = 0f;
        }
    }
}