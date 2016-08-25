using UnityEngine;
using System.Collections;

public class ComputeShaderFun : MonoBehaviour {


    
    public ComputeShader shader;
    float[] array; 
	// Use this for initialization
	void Start () {
        array = new float[36];
        for (int i = 0; i < 36;i++)
        {
            array[i] = i;
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void RunShader()
    {
        int kernelHandle = shader.FindKernel("CSMain");

        RenderTexture tex = new RenderTexture(256, 256, 24);
        tex.enableRandomWrite = true;
        tex.Create();

        shader.SetTexture(kernelHandle, "Result", tex);
        shader.Dispatch(kernelHandle, 256 / 8, 256 / 8, 1);
    }
}
