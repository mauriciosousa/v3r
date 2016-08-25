using UnityEngine;
using System.Collections;

public class TestShit : MonoBehaviour {

    Vector3[,] current3dPixels;
    Vector3[,] past3dPixels;
    public float d = 2.5f;
    public Vector3 p;
    public Vector3 pViewport;
    public GameObject cube;
    public Camera camera;
    public Vector3 temp;

	// Use this for initialization
	void Start () {
        current3dPixels = new Vector3[Screen.width, Screen.height];
        past3dPixels = new Vector3[Screen.width, Screen.height];
	}
	

    void compute3DDistances()
    {
        past3dPixels = current3dPixels;
        current3dPixels = new Vector3[Screen.width, Screen.height];

        for (int i = 0; i < Screen.width; i++)
        {
            for (int j = 0; j < Screen.height; j++)
            {
                temp = camera.ScreenToWorldPoint(new Vector3(i, j, camera.nearClipPlane));
                current3dPixels[i, j] = temp;

                float speed = ( current3dPixels[i, j].sqrMagnitude - past3dPixels[i, j].sqrMagnitude ) / Time.deltaTime;
                float ttc = current3dPixels[i, j].z / speed;
                float ttc2 = Mathf.Exp(-Mathf.Pow(ttc, 2) / (2*Mathf.Pow(d,2)));
            }

        }
    }

	// Update is called once per frame
	void Update () {

        /* */
        compute3DDistances();
        Debug.Log("blabla"); 

       /* Camera camera = GetComponent<Camera>();
        p = camera.ScreenToWorldPoint(new Vector3(0, 0, camera.nearClipPlane+10));
        pViewport = new Vector3(0, 0, camera.nearClipPlane);
        cube.transform.position = p;*/
        // Debug.Log(" Vector 3 = "+)
    }

    
}
