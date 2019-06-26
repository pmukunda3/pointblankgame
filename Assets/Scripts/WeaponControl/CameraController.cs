using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    //public float Scroll_Sensitivity = 1f;

    public float MaxInclination = 90f;
    public float MinInclination = -90f;
    public float CameraDistance = 7f;
    
    private Vector3 NewPosition, Offset;
    private Quaternion NewRotation;
    private float MouseYPos, Lat;

    void Start()
    {
        transform.localScale = CameraDistance * Vector3.one;
        transform.localPosition = new Vector3(0, CameraDistance/4, 0);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        MouseYPos = Input.mousePosition.y / Screen.height;
        Lat = Mathf.Lerp(MaxInclination, MinInclination, MouseYPos);
        transform.localEulerAngles = new Vector3(Lat, 0, 0);
    }
}
