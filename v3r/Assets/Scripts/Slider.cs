using UnityEngine;
using System.Collections;

public class Slider : MonoBehaviour {

    [Range(0, 1)]
    public float percentage = 0f;

    [Range(0, 1)]
    public float opacity = 0f;

    private Renderer _renderer;
    private float _mainTextureScale;

    void Start () {
        _renderer = GetComponent<Renderer>();
        _mainTextureScale = _renderer.material.mainTextureScale.y;
	}
	
	void Update () {
        _renderer.material.mainTextureOffset = new Vector2(0f, _mainTextureScale - percentage * _mainTextureScale);
        Color color = _renderer.material.color;
        color.a = opacity;
        _renderer.material.color = color;
	}
}
