using UnityEngine;
using System.Collections.Generic;

public class TouchController : MonoBehaviour {

    Vector2 lastTouchCenter;
    int lastTouchCount;
    float lastDistance;
    Dictionary<int, Vector2> lastTouchPositions;

    public GameObject obj;

    private UdpBroadcast udpBroadcast;

	// Use this for initialization
	void Start ()
    {
        lastTouchCount = 0;
        lastTouchPositions = new Dictionary<int, Vector2>();

        udpBroadcast = new UdpBroadcast(11911);
    }
	
	// Update is called once per frame
	void Update ()
    {
        string msg = "V3R/S=" + 1 + ";" + 1;

        if (Input.touchCount > 0)
        {
            msg += "/RH=" + Input.touchCount + ";";

            // Calc new center

            Vector2 tMax = Input.GetTouch(0).position;
            Vector2 tMin = Input.GetTouch(0).position;

            for (int i = 1; i < Input.touchCount; i++)
            {
                Vector2 tPos = Input.GetTouch(i).position;
                tMax = new Vector2(Mathf.Max(tMax.x, tPos.x), Mathf.Max(tMax.y, tPos.y));
                tMin = new Vector2(Mathf.Min(tMin.x, tPos.x), Mathf.Min(tMin.y, tPos.y));
            }

            Vector2 touchCenter = (tMin + tMax) / 2.0f;

            // Calc avg touch distance to touch center

            float avgDistance = 0;
            for (int i = 0; i < Input.touchCount; i++)
            {
                avgDistance += (touchCenter - Input.GetTouch(i).position).magnitude;
            }
            avgDistance /= (float)Input.touchCount;

            // Calc avg touch rotation around touch center

            float avgRotation = 0;
            if (lastTouchCount == Input.touchCount && Input.touchCount > 1)
            {
                for (int i = 0; i < Input.touchCount; i++)
                {
                    Vector2 oldDir = lastTouchPositions[Input.GetTouch(i).fingerId] - lastTouchCenter;
                    Vector2 newDir = Input.GetTouch(i).position - touchCenter;
                    float angle = Vector2.Angle(oldDir, newDir);
                    if (Vector3.Cross(oldDir, newDir).z < 0) angle = -angle;
                    avgRotation += angle;
                }
                avgRotation /= (float)Input.touchCount;
            }

            // Calc scale and rotation

            if (lastTouchCount == Input.touchCount)
            {
                // scale

                if (Input.touchCount > 1)
                {
                    float scale = avgDistance / lastDistance;
                    obj.transform.localScale *= scale;
                }

                // rotate

                obj.transform.Rotate(0, 0, avgRotation);
            }

            if (lastTouchCount == 0)
            {
                obj.transform.localScale = new Vector3(1, 1, 1);
                obj.transform.rotation = Quaternion.identity;
                obj.transform.position = Vector3.zero;// new Vector3((touchCenter.x / (float)Screen.height - ((Screen.width / (float)Screen.height) / 2.0f)), touchCenter.y / (float)Screen.height - 0.5f, 0) * 2.0f;
            }
            else if (lastTouchCount == Input.touchCount)
            {
                Vector2 touchCenterDelta = touchCenter - lastTouchCenter;
                obj.transform.position += new Vector3(touchCenterDelta.x / (float)Screen.height, touchCenterDelta.y / (float)Screen.height, 0) * 2.0f;
            }

            msg += obj.transform.position.x + ";" + obj.transform.position.y + ";" + obj.transform.eulerAngles.z + ";" + obj.transform.localScale.x;

            lastTouchCenter = touchCenter;
            lastDistance = avgDistance;

            obj.SetActive(true);
        }
        else
            obj.SetActive(false);

        lastTouchPositions.Clear();
        for (int i = 0; i < Input.touchCount; i++)
        {
            lastTouchPositions[Input.GetTouch(i).fingerId] = Input.GetTouch(i).position;
        }
        lastTouchCount = Input.touchCount;

        print(msg);
        udpBroadcast.send(msg);
    }

    void OnGUI()
    {

    }
}
