using UnityEngine;
using System.Collections;
using System;

public enum LeftHandStates
{
	BRIGHTNESS,
	SLICES,
	NONE
}

public class SimpleObjController : MonoBehaviour {

	private LeftHandStates leftHandState = LeftHandStates.NONE;

    public RayMarching volume;
	public Renderer volumeRenderer;
    public int rotationMax;

    private string touchMessage;

    private int touchCountRight;
	private int touchCountLeft;
    private Vector2 position;
    private float rotation;
    private float scale;

    private Vector3 _resetPosition;
    private Quaternion _resetRotation;
    private Vector3 _resetScale;


	public Slider brightnessSlider;
	public Slider slicesSlider;
	[Range(0,1)]
	public float SlidersNoTouchOpacity;

	public GameObject RightHandNob;
	private Vector3 _rightHandInitialPosition;
	public GameObject LeftHandNob;
	private Vector3 _leftHandInitialPosition;

	[Range(0.1f, 1f)]
	public float nobPositionFactor = 0.1f;


	public float SlicesVelocity = 20f;
	public float BrightnessVelocity = 0.1f;

	public GameObject desk;

	void Start () {
        touchMessage = null;

        _resetPosition = transform.position;
        _resetRotation = transform.rotation;
        _resetScale = transform.localScale;

		_rightHandInitialPosition = RightHandNob.transform.position;
		_leftHandInitialPosition = LeftHandNob.transform.position;
	}
	
	void Update ()
    {
        _keyboardInput();

		LeftHandNob.transform.position = _leftHandInitialPosition;
		RightHandNob.transform.position = _rightHandInitialPosition;


        // Parse Message
        if (touchMessage != null)
        {
            string[] values = touchMessage.Split('/');
            touchMessage = null;



            if (values[0] == "V3R")
            {
                bool RHmissing = true;
				bool LHmissing = true;

                for (int i = 1; i < values.Length; i++)
                {
                    string [] st = values[i].Split('=');
                    if (st[0] == "RH")
                    {
                        RHmissing = false;

                        string[] v = st[1].Split(';');
                        int tc = int.Parse(v[0]);
                        Vector2 p = new Vector2(float.Parse(v[1]), float.Parse(v[2]));
                        float r = float.Parse(v[3]);
                        float s = float.Parse(v[4]);

						_changeHandDeskPosition(RightHandNob, p, r, s);

                        if(tc == touchCountRight)
                        {
                            Vector3 pos = transform.position;
                            Vector3 rot = transform.eulerAngles;
                            float sca = transform.localScale.x;



                            pos.x += p.x - position.x;
							pos.x = Mathf.Clamp(pos.x, - desk.transform.localScale.x / 2, desk.transform.localScale.x / 2);
							pos.z += p.y - position.y;
							rot.y -= r - rotation;
                            sca += s - scale;


                            if(rot.y > 180)
                            {
                                if(rot.y < (360 - rotationMax))
                                {
                                    rot.y = 360 - rotationMax;
                                }
                            }
                            else
                            {
                                if(rot.y > rotationMax)
                                {
                                    rot.y = rotationMax;
                                }
                            }

                            sca = Mathf.Clamp(sca, 0.5f, 2.0f);

                            transform.position = pos;
                            transform.eulerAngles = rot;
							transform.localScale = new Vector3(sca, sca, sca);
						
                        }


						if (volumeRenderer.bounds.min.z < desk.GetComponent<Renderer>().bounds.max.z) 
						{
							transform.position = transform.position + new Vector3(0, 0, desk.GetComponent<Renderer>().bounds.max.z - volumeRenderer.bounds.min.z);
						}

                        touchCountRight = tc;
                        position = p;
                        rotation = r;
                        scale = s;
                    }
                    else if (st[0] == "LH")
                    {
						LHmissing = false;

						string[] v = st[1].Split(';');
						int tc = int.Parse(v[0]);
						Vector2 p = new Vector2(float.Parse(v[1]), float.Parse(v[2]));
						float r = float.Parse(v[3]);
						float s = float.Parse(v[4]);

						_changeHandDeskPosition(LeftHandNob, p, r, s);

						if(tc == touchCountLeft)
						{

							float xd = (p.x - position.x);
							float yd = (p.y - position.y);

							if (leftHandState == LeftHandStates.NONE)
							{
								if (Mathf.Abs(xd) > Mathf.Abs(yd))
									leftHandState = LeftHandStates.BRIGHTNESS;
								else 
									leftHandState = LeftHandStates.SLICES;
							}
							else if (leftHandState == LeftHandStates.BRIGHTNESS)
							{
								if (xd != 0)
								{
									brightnessSlider.opacity = 1f;
									volume.bright += (xd > 0 ? 1 : -1) * BrightnessVelocity * Time.deltaTime;
									volume.bright = Mathf.Clamp(volume.bright, 0, 1);
								}
								else brightnessSlider.opacity = SlidersNoTouchOpacity;
							}
							else if (leftHandState == LeftHandStates.SLICES)
							{
								if (yd != 0 ) 
								{
									slicesSlider.opacity = 1f;
									volume.clipDimensions2.z += (yd > 0 ? 1 : -1) * SlicesVelocity * Time.deltaTime;
									volume.clipDimensions2.z = Mathf.Clamp(volume.clipDimensions2.z, 0,99);
								}
								else slicesSlider.opacity = SlidersNoTouchOpacity;	
							}
						}
						touchCountLeft = tc;
                    }
                }

                if (RHmissing)
                {
                    touchCountRight = 0;
                    position = Vector2.zero;
                    rotation = 0;
                    scale = 1;
                }

				if (LHmissing)
				{
					touchCountLeft = 0;
					brightnessSlider.opacity = SlidersNoTouchOpacity;
					slicesSlider.opacity = SlidersNoTouchOpacity;
					leftHandState = LeftHandStates.NONE;
				}


				setGameobjectVisibility(RightHandNob, !RHmissing);
				setGameobjectVisibility(LeftHandNob, !LHmissing);

            }
        }

		_updateDesk();

    }

	private void _changeHandDeskPosition(GameObject go, Vector2 position, float rotation, float scale)
	{
		Vector3 init = (go.name == "HandRightNob") ? _rightHandInitialPosition : _leftHandInitialPosition;

		go.transform.position = init + new Vector3(nobPositionFactor * position.x, 0, nobPositionFactor * position.y);


		Vector3 r = go.transform.eulerAngles;
		r.y =  - rotation;
		go.transform.eulerAngles = r;

		go.transform.localScale = new Vector3(1 + 0.1f * scale, 1, 1 + 0.1f * scale);

	}

	private void _updateDesk()
	{
		slicesSlider.percentage = 1 - Mathf.Clamp(volume.clipDimensions2.z / 100, 0, 1);
		brightnessSlider.percentage = Mathf.Clamp(volume.bright, 0, 1);
	}
		

    private void _keyboardInput()
    {
        // position

        Vector3 pos = transform.position;

        if (Input.GetKey(KeyCode.R))
        {
            resetVolume();
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            pos.y += 0.2f * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            pos.y -= 0.2f * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            pos.x += 0.2f * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            pos.x -= 0.2f * Time.deltaTime;
        }

        transform.position = pos;

        // rotation

        Vector3 rot = transform.eulerAngles;

        if (Input.GetKey(KeyCode.Z))
        {
            rot.y += 20.0f * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.X))
        {
            rot.y -= 20.0f * Time.deltaTime;
        }

        transform.eulerAngles = rot;

        // scale

        Vector3 scale = transform.localScale;

        if (Input.GetKey(KeyCode.C))
        {
            scale.x += 0.5f * Time.deltaTime;
            scale.y += 0.5f * Time.deltaTime;
            scale.z += 0.5f * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.V))
        {
            scale.x -= 0.5f * Time.deltaTime;
            scale.y -= 0.5f * Time.deltaTime;
            scale.z -= 0.5f * Time.deltaTime;
        }

        transform.localScale = scale;

        // slices

        if (Input.GetKey(KeyCode.W))
        {
			volume.clipDimensions2.z = Mathf.Clamp(volume.clipDimensions2.z + 10.0f * Time.deltaTime, 1, 99);
        }

        if (Input.GetKey(KeyCode.S))
        {
			volume.clipDimensions2.z = Mathf.Clamp(volume.clipDimensions2.z - 10.0f * Time.deltaTime, 1, 99);
        }

        // brightness

        if (Input.GetKey(KeyCode.A))
        {
			if (volume.bright > 0) volume.bright -= 0.5f * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.D))
        {
			if (volume.bright < 1) volume.bright += 0.5f * Time.deltaTime;
        }

		// opacity

		if (Input.GetKey(KeyCode.O))
		{
			if (volume.opacity > 0) volume.opacity -= 0.1f * Time.deltaTime;
		}

		if (Input.GetKey(KeyCode.L))
		{
			if (volume.opacity < 1) volume.opacity += 0.1f * Time.deltaTime;
		}
    }

    private void resetVolume()
    {
        transform.position = _resetPosition;
        transform.rotation = _resetRotation;
        transform.localScale = _resetScale;
    }

    public void setNewTouchMessage(string message)
    {
        touchMessage = message;
    }

	public void setGameobjectVisibility(GameObject go, bool active)
	{
		foreach( Transform child in go.transform )
		{
			child.gameObject.SetActive(active);
		}
	}

	public bool inRange(float value, float bottom, float top)
	{
		return (value > bottom && value < top);
	}
}
