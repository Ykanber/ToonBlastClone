using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public void SetupCamera(int Width, int Height)
    {
        SetupCameraPosition(Width, Height);
        SetupCameraOrthographicSize(Width, Height);
    }

    /// <summary>
    /// My grid always starts at 0,0 position, In here I am centering the camera
    /// </summary>
    /// 
    void SetupCameraPosition(int Width, int Height)
    {
        Vector3 cameraPos = Camera.main.transform.position;
        cameraPos = new Vector3((cameraPos.x) + Width / 2f, (cameraPos.y) + 1 + Height / 2f, cameraPos.z);
        Camera.main.transform.position = cameraPos;
    }

    /// <summary>
    /// I am finding the correct value for camera orthographic size, if the phone is not 9:16 the results can be wrong
    /// </summary>
    void SetupCameraOrthographicSize(int Width, int Height)
    {
        float size;
        if (Width > Height * 9 / 16)
        {
            size = (Width * 1.7f) / 2f + 3;
        }
        else
        {
            size = Height / 2f + 3;
        }
        Camera.main.orthographicSize = size;
    }
}
