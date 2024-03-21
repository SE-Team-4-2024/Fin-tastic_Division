using UnityEngine;
using UnityEngine.UI;

public class Scroller : MonoBehaviour {
    [SerializeField] private RawImage _img;
    [SerializeField] private float _scrollSpeed;

    private float _offset;

    void Start() {
        _offset = 0f;
    }

    void Update() {
        _offset += _scrollSpeed * Time.deltaTime;

        if (_offset > 1.0f)
            _offset -= 1.0f;

        _img.uvRect = new Rect(_offset, 0, 1, 1);
    }
}
