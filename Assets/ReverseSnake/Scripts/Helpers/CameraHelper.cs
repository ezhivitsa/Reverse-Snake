using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.src;

public class CameraHelper : MonoBehaviour
{
    void Awake ()
    {
        float aspectRatio = (float)Screen.currentResolution.height / (float)Screen.currentResolution.width;
        if (aspectRatio <= SetupConstants.Resolution3k2)
        {
            Camera.main.orthographicSize = SetupConstants.OrthographicSize3k2;
        }
        else if (aspectRatio <= SetupConstants.Resolution16k9)
        {
            Camera.main.orthographicSize = SetupConstants.OrthographicSize16k9;
        }
        else if (aspectRatio <= SetupConstants.Resolution18k9)
        {
            Camera.main.orthographicSize = SetupConstants.OrthographicSize18k9;
        }
        else
        {
            Camera.main.orthographicSize = SetupConstants.OrthographicSize19k9;
        }
    }
}
