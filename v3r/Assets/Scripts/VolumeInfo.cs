using UnityEngine;
using System.Collections;

public class VolumeInfo : MonoBehaviour {

    public Renderer volumeRenderer;
    public SimpleObjController simpleObjectController;
    public RayMarching volume;

    void Start () {
	
	}
	
	void Update () {
        //transform.LookAt(Camera.main.transform.position);


        Vector3 ori = volumeRenderer.bounds.center;
        float radius = volumeRenderer.bounds.extents.magnitude;
        //ori += new Vector3(0, 0, radius);

        //Debug.DrawRay(ori, Camera.main.transform.position - ori, Color.red);
        //Ray ray = new Ray(ori, Camera.main.transform.position - ori);

        //Plane hPlane = new Plane(Vector3.forward, new Vector3(0, 0, 0));
        //float distance = 0;
        //if (hPlane.Raycast(ray, out distance))
        //{
        //    transform.position = ray.GetPoint(distance);
        //}

        //Vector3 p = ori + new Vector3(radius, -radius / 3, 0);
        //p.z = -0.1f;
        //transform.position = p;
        //transform.forward = transform.position - Camera.main.transform.position;

        string s = "";

        s = "" + simpleObjectController.volumeFace;

        float v = 0;

        if (simpleObjectController.volumeFace == CubemapFace.PositiveX)
        {
            s = "Sagittal Right";
            v = volume.clipDimensions.x;
        }
        else if (simpleObjectController.volumeFace == CubemapFace.NegativeX)
        {
            s = "Sagittal Left";
            v = volume.clipDimensions2.x;
        }
        else if (simpleObjectController.volumeFace == CubemapFace.NegativeZ)
        {
            s = "Horizontal Top";
            v = volume.clipDimensions.z;
        }
        else if (simpleObjectController.volumeFace == CubemapFace.PositiveZ)
        {
            s = "Horizontal Bottom";
            v = volume.clipDimensions2.z;
        }

        s += " View";


        s += "\nSlice: " + volume.slices.Length * ((int)Mathf.Clamp(v, 0, 100)) / 100;


        gameObject.GetComponent<TextMesh>().text = s;
    }
}
