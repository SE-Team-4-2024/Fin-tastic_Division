using UnityEngine;

public class SeaSurfaceAnimation : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer; // Reference to the SpriteRenderer component
    [SerializeField] private float _scrollSpeed;

    private float _leftBound; // Left boundary of the SpriteRenderer
    private float _rightBound; // Right boundary of the SpriteRenderer

    void Start()
    {
        // Calculate the extreme left and right points of the SpriteRenderer
        _leftBound = _spriteRenderer.bounds.min.x;
        _rightBound = _spriteRenderer.bounds.max.x;
    }

    void Update()
    {
        // Move the SpriteRenderer from right to left
        transform.Translate(Vector3.left * _scrollSpeed * Time.deltaTime);

        // Check if the SpriteRenderer's extreme right point has passed the left boundary
        if (transform.position.x < _leftBound)
        {
            // Move the SpriteRenderer to the right boundary
            transform.position = new Vector3(_rightBound, transform.position.y, transform.position.z);
        }
    }
}
