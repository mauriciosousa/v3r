using UnityEngine;
using System.Collections;

public class SliceNumber : MonoBehaviour {

    public RayMarching volume;
    public SimpleObjController simpleObjectController;

	void Start () {
	    
	}
	
	void Update () {

        float v = 0;

        if (simpleObjectController.volumeFace == CubemapFace.PositiveX)
        {
            v = volume.clipDimensions.x;
        }
        else if (simpleObjectController.volumeFace == CubemapFace.NegativeX)
        {
            v = volume.clipDimensions2.x;
        }
        else if (simpleObjectController.volumeFace == CubemapFace.NegativeZ)
        {
            v = volume.clipDimensions.z;
        }
        else if (simpleObjectController.volumeFace == CubemapFace.PositiveZ)
        {
            v = volume.clipDimensions2.z;
        }

        int slice = volume.slices.Length * ((int)Mathf.Clamp(v, 0, 100)) / 100;

		gameObject.GetComponent<TextMesh>().text = "" + (slice > 0 ? slice : 1);
	}
}
