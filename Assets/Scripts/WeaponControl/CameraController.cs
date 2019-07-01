using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    //public float Scroll_Sensitivity = 1f;

    public float Y_Sensitivity = 1f; 
    public float MaxXRot = 90f;
    public float MinXRot = -90f;
    public float CameraDistance = 5f;
    public Vector3 PosOffset;
    public float YRotOffset = 0f;
    public bool AimWeapon = true;
    public float AimDistance = 100f;
    public Transform WeaponPivot;

    public Vector3 target
    {
        get;
        private set;
    }

    private Transform Camera;
    private Vector3 NewPosition, Offset;
    private Quaternion NewRotation;
    private float InputY, XRot;

    private void SetTarget()
    {
        if (Physics.Raycast(Camera.position, Camera.forward, out RaycastHit hit, AimDistance))
        {
            target = hit.point;
        }
        else
        {
            target = Camera.position + AimDistance * Camera.forward;
        }
    }

    void Start()
    {
        InputY = 0.5f;
        Camera = transform.GetChild(0);
        transform.localScale = CameraDistance * Vector3.one;
        transform.localPosition = CameraDistance * PosOffset;
    }

    // Update is called once per frame
    void Update()
    {
        InputY = Mathf.Clamp01(InputY + Input.GetAxis("Mouse Y") * Y_Sensitivity);
        XRot = Mathf.Lerp(MaxXRot, MinXRot, InputY);
        transform.localEulerAngles = new Vector3(XRot, YRotOffset, 0);
        if (AimWeapon)
        {
            SetTarget();
            WeaponPivot.LookAt(target);
        }
        
    }
}
