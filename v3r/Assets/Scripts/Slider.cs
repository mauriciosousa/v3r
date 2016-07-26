using UnityEngine;
using System.Collections;

public class Slider : MonoBehaviour {

    [Range(0, 1)]
    public float percentage = 0f;

    [Range(0, 1)]
    public float opacity = 0f;

    private Renderer _renderer;
    private float _mainTextureScale;

	public GameObject sliderIcon;
	private Renderer _sliderIconRenderer;

    void Start () {
        _renderer = GetComponent<Renderer>();
		_sliderIconRenderer = sliderIcon.GetComponent<Renderer>();
        _mainTextureScale = _renderer.material.mainTextureScale.y;
	}
	
	void Update () {
        _renderer.material.mainTextureOffset = new Vector2(0f, _mainTextureScale - percentage * _mainTextureScale);
		_changeOpacity(_renderer);
		_changeOpacity(_sliderIconRenderer);
	}

	private void _changeOpacity(Renderer renderer)
	{
		Color color = renderer.material.color;
		color.a = opacity;
		renderer.material.color = color;
	}
}
