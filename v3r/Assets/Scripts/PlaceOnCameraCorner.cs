using UnityEngine;
using System.Collections;

/// <summary>
/// Places an object near a camera such that it always appears in the corner
/// from the point of view of the camera.
/// This script is not all that sophisticated...
/// 
/// GameObject of this script must be a child of a GameObject with a Camera.
/// </summary>
[ExecuteInEditMode]
public class PlaceOnCameraCorner : MonoBehaviour
{
    public Camera parentCamera;
    /// <summary>
    /// Where the object should appear, in viewport space (values 0f-1f).
    /// </summary>
    public Vector2 viewportPoint = new Vector2(0.85f, 0.3f);
    public float size = 4f;

    // Use this for initialization
    void Start()
    {
        parentCamera = transform.parent.GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (parentCamera != null)
        {
            Ray ray = parentCamera.ViewportPointToRay(viewportPoint);

            if (!parentCamera.orthographic)
            {
                transform.position = ray.origin + ray.direction * size * 12;
                transform.localScale = Vector3.one * size * parentCamera.fieldOfView / 60f;
            }
            else
            {
                transform.position = ray.origin + ray.direction * 5;
                transform.localScale = Vector3.one * size * parentCamera.orthographicSize / 5.8f;
            }
        }
    }
}
