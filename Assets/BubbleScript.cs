using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleScript : MonoBehaviour
{
    private float _speed;
    private float _endposX;

    public void startFloating(float speed, float endposX)
    {
        _speed = speed;
        _endposX = endposX;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.up * (Time.deltaTime * _speed));
        if(transform.position.x > _endposX);
        {
            //Destroy(gameObject);
        }
    }
}
