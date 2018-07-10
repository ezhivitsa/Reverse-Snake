using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHelper : MonoBehaviour
{
    void Awake ()
    {
        float aspectRatio = (float)Screen.currentResolution.height / (float)Screen.currentResolution.width;
        if (aspectRatio <= 1.5f)
        {
            Camera.main.orthographicSize = 21.0f;
        }
        else if (aspectRatio <= 1.8f)
        {
            Camera.main.orthographicSize = 24.8f;
        }
        else if (aspectRatio <= 2.0f)
        {
            Camera.main.orthographicSize = 27.5f;
        }
        else
        {
            Camera.main.orthographicSize = 29.0f;
        }
    }
}
