using UnityEngine;
using System.Collections;

public class SliceNumber : MonoBehaviour {

    public RayMarching volume;

	void Start () {
	    
	}
	
	void Update () {

        int slice = volume.slices.Length * ((int)Mathf.Clamp(volume.clipDimensions2.z, 0, 100)) / 100;

		gameObject.GetComponent<TextMesh>().text = "" + (slice > 0 ? slice : 1);
	}
}
