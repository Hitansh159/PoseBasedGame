using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.IO;
using System.Threading;

public class cam : MonoBehaviour
{
    bool starting = true;
    static WebCamTexture backCam;
    int count = 0;
    GameObject[] points = new GameObject[17];
    float [,] result = new float[17, 3];
    public GameObject p;
    Texture2D texture;

    // Start is called before the first frame update
    void Start()
    {
        texture = new Texture2D(192, 192);
        for (int i = 0;i < 17; i++)
        {
            points[i] = GameObject.CreatePrimitive(PrimitiveType.Cube);
            points[i].transform.position = Vector3.zero;
            points[i].transform.localScale = Vector3.one * 0.02f;
        }


        WebCamDevice[] devices = WebCamTexture.devices;
        for (int i = 0; i < devices.Length; i++)
            Debug.Log(devices[i].name+" "+ devices[i].availableResolutions);

        if (backCam == null)
            backCam = new WebCamTexture(devices[0].name,192,192);
        
        GetComponent<Renderer>().material.mainTexture = backCam;
        
        if (!backCam.isPlaying)
            backCam.Play();


    }

    // Update is called once per frame
    void Update()
    {


            int size = 192; 
            Color[] Uimg = new Color[size * size];
            Uimg = backCam.GetPixels(0, 0, 120, 120);
            
            float[,,] img = new float[size, size, 3];
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    
                    try
                    {
                        img[i, j, 0] = i >= 120 || j >= 120 ? 0 : Uimg[i * (120 - 1) + j].r;
                        img[i, j, 1] = i >= 120 || j >= 120 ? 0 : Uimg[i * (120 - 1) + j].g;
                        img[i, j, 2] = i >= 120 || j >= 120 ? 0 : Uimg[i * (120 - 1) + j].b;
                        Color c = new Color(img[i, j, 0], img[i, j, 1], img[i, j, 2]);
                        texture.SetPixel(i, j, c);

                    }
                    catch
                    { 
                        Debug.Log(i+" "+j);
                    }
                    
                }
            }
           /* p.GetComponent<Renderer>().material.mainTexture = texture; 
            GetComponent<MoveNet>().Infer(img);
            result = GetComponent<MoveNet>().GetResult();
            DrawResult();
        */
    }

    private void DrawResult()
    {
        for(int i = 0; i < 17; i++)
        {
            points[i].transform.position = new Vector3(result[i, 0], result[i, 1], 0); 
        }        
    }
}
