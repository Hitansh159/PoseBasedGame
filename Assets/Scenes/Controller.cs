using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Controller : MonoBehaviour
{
    public bool calibrated = false;
    float lowerLine, upperLine;
    public float bandWidth = 0.2f;
    BlazePoseSample blazePose;
    private Vector2[] hipPosition;
    public int hipPositionCount = 0;
    private Vector2 meanHipPostion = Vector2.zero;
    string status;
    public Text statusText;

    public bool up = false, down = false;

    // Start is called before the first frame update
    void Start()
    {
        blazePose = GetComponent<BlazePoseSample>();
        hipPosition = new Vector2[48];
    }

    // Update is called once per frame
    void Update()
    {
        
        if (!calibrated)
        {
            status = "Calibrating";
            Calibrate();
        }
        else
        {
            hipPositionCount++;
            hipPositionCount %= 48;
            hipPosition[hipPositionCount] = (blazePose.landmarkResult.viewportLandmarks[23] + blazePose.landmarkResult.viewportLandmarks[24]) / 2;
            if(hipPosition[hipPositionCount].y < lowerLine)
            {
                status = "Down";
                down = true;
                up = false;
            }
            else if(hipPosition[hipPositionCount].y > upperLine)
            {
                status = "Up";
                up = true;
                down = false;
            }
            else
            {
                status = "Idel";
                up = down = false; 
            }

        }
        statusText.text = status;
        

    }

    void Calibrate()
    {
        if (blazePose.landmarkResult == null || blazePose.landmarkResult.score ==  0) return;
        Vector2 hip = (blazePose.landmarkResult.viewportLandmarks[23] + blazePose.landmarkResult.viewportLandmarks[24]) / 2;
        hipPosition[hipPositionCount] = hip;
        hipPositionCount++;
        meanHipPostion.x += hip.x;
        meanHipPostion.y += hip.y;

        if(hipPositionCount == 48)
        {
            meanHipPostion = new Vector2(meanHipPostion.x / 48, meanHipPostion.y / 48);
            lowerLine = meanHipPostion.y - bandWidth / 2;
            upperLine = meanHipPostion.y + bandWidth / 2;

            if(upperLine < 0.2f || lowerLine > 0.5f)
            {
                Debug.Log(upperLine + " " + lowerLine);
                meanHipPostion = Vector2.zero;
                hipPositionCount = 0;
                if (upperLine > -0.5f)
                {
                    status = "Go back";
                }
                else if(lowerLine < -0.5f)
                {
                    status = "Come Forward";
                }
            }
            else
            {
                calibrated = true;
            }

        }
    }
}
