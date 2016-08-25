using UnityEngine;
using System.Collections;

public class shaderFun : MonoBehaviour {

    public Renderer rend;
    public Texture text;

	// Use this for initialization
	void Start () {
        rend = GetComponent<Renderer>();
       
        for(int i = 0; i < 36;i++)
        {
            rend.material.SetFloat("flatArray" + i, i);
        }
        string tmpStr = "ARRAY = ";
        for(int i = 0; i < 36;i++)
        {
            tmpStr += rend.material.GetFloat("flatArray" + i)+" ";
        }
        Debug.Log(tmpStr);
    }
	
	// Update is called once per frame
	void Update () {
        Debug.Log("d");
        string tmpString = "ARRAY AFTER DEFLATTEN = ";

        for (int i = 0; i < 36; i++)
        {
            tmpString += rend.material.GetFloat("flatArray" + i)+" ";
        }
        Debug.Log(tmpString);
    }
}
