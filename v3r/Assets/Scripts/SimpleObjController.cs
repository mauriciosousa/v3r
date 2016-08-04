using UnityEngine;
using System.Collections;
using System;

public enum LeftHandStates
{
    TOUCH,
	BRIGHTNESS,
	SLICES,
	NONE
}

public class SimpleObjController : MonoBehaviour {

    private string debugMessage;

    private string tapDebug = "";

    private LeftHandStates leftHandState = LeftHandStates.NONE;

    public bool keyboardSupport;

    public RayMarching volume;
    public CubemapFace volumeFace = CubemapFace.PositiveZ;
	public Collider volumeRenderer;
    public int rotationMax;

    public Transform Canvas;

    private string touchMessage;
    private string touchMessageGUI;

    private int touchCountRight;
	private int touchCountLeft;
    private Vector2 positionRight;
    private Vector2 positionLeft;
    private float rightRotation;
    private float leftRotation;
    private float scale;

    [SerializeField]
    private bool recenter = false;
    private Quaternion recenterRotation;
    public float lerpTimeMax;

    private Vector3 _resetPosition;
    private Quaternion _resetRotation;
    private Vector3 _resetScale;
    private Vector3 _resetClip1;
    private Vector3 _resetClip2;

    public Slider brightnessSlider;
	public Slider slicesSlider;
	[Range(0,1)]
	public float SlidersNoTouchOpacity;

	public GameObject RightHandNob;
	private Vector3 _rightHandInitialPosition;
    private float rightHandTravel = 0;
	public GameObject LeftHandNob;
	private Vector3 _leftHandInitialPosition;
    private float leftHandRotationTravel = 0;

	[Range(0.1f, 1f)]
	public float nobPositionFactor = 0.1f;

	public float SliderFactor = 1f;

	public GameObject desk;

    private float lerptime = 0;


	private bool _vrmode = true;
	private Vector3 _vrCameraPosition;
	private Quaternion _vrCameraRotation;

	void Start () {
        touchMessage = null;

        _resetPosition = transform.position;
        _resetRotation = transform.rotation;
        _resetScale = transform.localScale;
        _resetClip1 = volume.clipDimensions;
        _resetClip2 = volume.clipDimensions2;

		_rightHandInitialPosition = RightHandNob.transform.position;
		_leftHandInitialPosition = LeftHandNob.transform.position;

		_vrCameraPosition = Camera.main.transform.position;
		_vrCameraRotation = Camera.main.transform.rotation;
	}
	
	void Update ()
    {
		if (Input.GetKey(KeyCode.R))
		{
			resetVolume();
		}
		if (Input.GetKeyDown(KeyCode.P))
		{
			if (_vrmode)
			{
				Camera.main.transform.position = new Vector3(0f, 0.3f, Camera.main.transform.position.z);
				Camera.main.transform.rotation = Quaternion.Euler(new Vector3(20f, 0f, 0f));
			}
			else
			{
				Camera.main.transform.position = _vrCameraPosition;
				Camera.main.transform.rotation = _vrCameraRotation;
			}
			_vrmode = !_vrmode;
		}

        if (keyboardSupport) _keyboardInput();

		LeftHandNob.transform.position = _leftHandInitialPosition;
		RightHandNob.transform.position = _rightHandInitialPosition;


        tapDebug = "";

        if (touchMessage != null) // Parse Message
        {
            string[] values = touchMessage.Split('/');
            touchMessageGUI = touchMessage;
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

                        if (tc == touchCountRight)
                        {
                            Vector3 pos = transform.position;
                            Vector3 rot = transform.localEulerAngles;
                            Vector3 rotCanvas = Canvas.rotation.eulerAngles;
                            float sca = transform.localScale.x;
                            pos.x += p.x - positionRight.x;
                            pos.x = Mathf.Clamp(pos.x, -desk.transform.localScale.x / 2, desk.transform.localScale.x / 2);
                            //pos.z += p.y - positionRight.y;
                            //pos.z = Mathf.Clamp(pos.z, 0, 1);
                            rot.y -= r - rightRotation;
                            rotCanvas.x -= 25f * -(p.y - positionRight.y);
                        
                            sca += s - scale;

                            if (p.y == positionRight.y)
                                rightHandTravel = 0f;
                            else
                                rightHandTravel += Mathf.Abs(p.y - positionRight.y);

                            



                            
                            if (rotCanvas.x <= 0) rotCanvas.x += 360;
                            else if (rotCanvas.x > 360) rotCanvas.x -= 360;

                            if (rotCanvas.x < 180)
                                rotCanvas.x = 0;
                            else if (rotCanvas.x < 360 - 20)
                                rotCanvas.x = 360 - 20;

                            /*
                            if (rot.y > 180)
                            {
                                if (rot.y < (360 - rotationMax))
                                {
                                    rot.y = 360 - rotationMax;
                                }
                            }
                            else
                            {
                                if (rot.y > rotationMax)
                                {
                                    rot.y = rotationMax;
                                }
                            }
                            */



                            sca = Mathf.Clamp(sca, 0.5f, 4.0f);
                            //transform.position = pos;

                            if (rightHandTravel > 0.01f)
                            {
                                Canvas.eulerAngles = rotCanvas;
                            }
                            else
                            {
                                transform.localEulerAngles = rot;
                                transform.localScale = new Vector3(sca, sca, sca);
                            }
                        }


                        if (volumeRenderer.bounds.min.z < desk.GetComponent<Renderer>().bounds.max.z - 0.10f)
                        {
                            transform.position += new Vector3(0, 0, desk.GetComponent<Renderer>().bounds.max.z - volumeRenderer.bounds.min.z);
                        }

                        touchCountRight = tc;
                        positionRight = p;
                        rightRotation = r;
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

                        if (tc == touchCountLeft)
                        {

                            float xd = (p.x - positionLeft.x);
                            float yd = (p.y - positionLeft.y);

                            if (leftHandState == LeftHandStates.NONE)
                            {
                                if (Mathf.Abs(p.x) > 0.1f)
                                    leftHandState = LeftHandStates.BRIGHTNESS;
                                else if (Mathf.Abs(p.y) > 0.1f)
                                    leftHandState = LeftHandStates.SLICES;
                            }
                            else if (leftHandState == LeftHandStates.BRIGHTNESS)
                            {
                                brightnessSlider.opacity = 1f;

                                volume.bright += xd * SliderFactor;
                                volume.bright = Mathf.Clamp(volume.bright, 0, 1);

                            }
                            else if (leftHandState == LeftHandStates.SLICES)
                            {
                                slicesSlider.opacity = 1f;
                                _cutSlice(yd * SliderFactor * 100.0f);
                            }

                            debugMessage = "" + r;
                        }
                        touchCountLeft = tc;
                        positionLeft = p;
                    }
                    else if (st[0] == "DOUBLETAP")
                    {
                        tapDebug = "DOUBLETAP";
                        resetVolumeClipping();
                    }
                    else if (st[0] == "TAP")
                    {
                        tapDebug = "TAP";
                        resetVolumeClippingCurrent();
                    }
                }


                _calcVolumeFace();
                Quaternion recenterTarget = Quaternion.identity;
                if (volumeFace == CubemapFace.PositiveX) recenterTarget = Quaternion.Euler(0, -90, 0);
                else if (volumeFace == CubemapFace.NegativeX) recenterTarget = Quaternion.Euler(0, 90, 0);
                else if (volumeFace == CubemapFace.NegativeZ) recenterTarget = Quaternion.Euler(0, 180, 0);
                else if (volumeFace == CubemapFace.PositiveZ) recenterTarget = Quaternion.Euler(0, 0, 0);

                if (RHmissing)
                {
                    rightHandTravel = 0;

                    if(touchCountRight > 0)
                    {
                        recenter = true;
                        recenterRotation = transform.localRotation;
                        lerptime = 0f;
                    }

                    touchCountRight = 0;
                    positionRight = Vector2.zero;
                    rightRotation = 0;
                    scale = 1;

                    if (recenter)
                    {
                        if (lerptime < lerpTimeMax)
                        {
                            lerptime += Time.deltaTime;
                            transform.localRotation = Quaternion.Lerp(recenterRotation, recenterTarget, lerptime / lerpTimeMax);
                        }
                        else
                        {
                            recenter = false;
                        }
                    }
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

        transform.position += new Vector3(0, desk.GetComponent<Renderer>().bounds.max.y - volumeRenderer.bounds.min.y, desk.GetComponent<Renderer>().bounds.max.z - volumeRenderer.bounds.min.z);

        _updateDesk();

    }

    private void _cutSlice(float v)
    {
        if (volumeFace == CubemapFace.PositiveX)
        {
            volume.clipDimensions.x -= v;
            volume.clipDimensions.x = Mathf.Clamp(volume.clipDimensions.x, 0, 99);
        }
        else if (volumeFace == CubemapFace.NegativeX)
        {
            volume.clipDimensions2.x += v;
            volume.clipDimensions2.x = Mathf.Clamp(volume.clipDimensions2.x, 0, 99);
        }
        else if (volumeFace == CubemapFace.NegativeZ)
        {
            volume.clipDimensions.z -= v;
            volume.clipDimensions.z = Mathf.Clamp(volume.clipDimensions.z, 0, 99);
        }
        else if (volumeFace == CubemapFace.PositiveZ)
        {
            volume.clipDimensions2.z += v;
            volume.clipDimensions2.z = Mathf.Clamp(volume.clipDimensions2.z, 0, 99);
        }
    }

    private void _calcVolumeFace()
    {
        float r = transform.rotation.eulerAngles.y;
        
        if (r > 45 && r <= 135)
        {
            volumeFace = CubemapFace.NegativeX;
        }
        else if (r > 135 && r <= 225)
        {
            volumeFace = CubemapFace.NegativeZ;
        }
        else if (r > 225 && r <= 315)
        {
            volumeFace = CubemapFace.PositiveX;
        }
        else if ((r >= 0 && r <= 45) || (r <= 360 && r > 315))
        {
            volumeFace = CubemapFace.PositiveZ;
        }
        else volumeFace = CubemapFace.Unknown;
    }

    private void _changeHandDeskPosition(GameObject go, Vector2 position, float rotation, float scale)
	{
		Vector3 init = (go.name == "HandRightNob") ? _rightHandInitialPosition : _leftHandInitialPosition;

		go.transform.position = init + new Vector3(nobPositionFactor * position.x, 0, nobPositionFactor * position.y);

		Vector3 r = go.transform.eulerAngles;
		r.y =  - rotation;
		go.transform.eulerAngles = r;

		go.transform.localScale = new Vector3(1 + 0.15f * scale, 1, 1 + 0.15f * scale);
	}

	private void _updateDesk()
	{
        if (volumeFace == CubemapFace.PositiveX)
        {
            slicesSlider.percentage = Mathf.Clamp(volume.clipDimensions.x / 100, 0, 1);
        }
        else if (volumeFace == CubemapFace.NegativeX)
        {
            slicesSlider.percentage = 1 - Mathf.Clamp(volume.clipDimensions2.x / 100, 0, 1);
        }
        else if (volumeFace == CubemapFace.NegativeZ)
        {
            slicesSlider.percentage = Mathf.Clamp(volume.clipDimensions.z / 100, 0, 1);
        }
        else if (volumeFace == CubemapFace.PositiveZ)
        {
            slicesSlider.percentage = 1 - Mathf.Clamp(volume.clipDimensions2.z / 100, 0, 1);
        }

        brightnessSlider.percentage = Mathf.Clamp(volume.bright, 0, 1);
	}
		
    private void resetVolume()
    {
        transform.position = _resetPosition;
        transform.rotation = _resetRotation;
        transform.localScale = _resetScale;
    }

    private void resetVolumeClipping()
    {
        volume.clipDimensions = _resetClip1;
        volume.clipDimensions2 = _resetClip2;
    }

    private void resetVolumeClippingCurrent()
    {
        if (volumeFace == CubemapFace.PositiveX)
        {
            volume.clipDimensions.x = _resetClip1.x;
        }
        else if (volumeFace == CubemapFace.NegativeX)
        {
            volume.clipDimensions2.x = _resetClip2.x;
        }
        else if (volumeFace == CubemapFace.NegativeZ)
        {
            volume.clipDimensions.z = _resetClip1.z;
        }
        else if (volumeFace == CubemapFace.PositiveZ)
        {
            volume.clipDimensions2.z = _resetClip2.z;
        }
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

    void OnGUI()
    {
        //GUIStyle style = new GUIStyle();
        //style.normal.textColor = Color.white;
        //style.fontSize = 100;
        //GUI.Label(new Rect(0, 0, Screen.width, Screen.height), tapDebug, style);
        //GUI.Label(new Rect(0, 0, 100, 100), "" + volumeFace);
        //GUI.Label(new Rect(0, 35, 100, 100), debugMessage);

        //GUI.Label(new Rect(0, 500, 1000, 1000), "Object: " + transform.rotation.eulerAngles);
        //GUI.Label(new Rect(0, 535, 1000, 1000), "Canvas: " + Canvas.transform.rotation.eulerAngles);
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
    }
}
