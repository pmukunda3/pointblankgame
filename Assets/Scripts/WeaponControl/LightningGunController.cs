using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FORGE3D;

public class LightningGunController : MonoBehaviour
{
    public GameObject beam1;
    public GameObject beam2;
    public GameObject impact;
    public float range = 100f;
    public float impactOffset = 0.5f;

    private F3DLightning beam1setting, beam2setting;
    private float collisionDistance;
    private float multiplier;

    /*
    private void SetBeamLength(float length)
    {
        beam1setting.MaxBeamLength = length - impactOffset;
        beam2setting.MaxBeamLength = length - impactOffset;
        multiplier = length / 10;
              
        beam1setting.beamScale = multiplier / 10;
        beam2setting.beamScale = multiplier / 10;
    }
    */

    // Start is called before the first frame update
    void Start()
    {
        //beam1setting = beam1.GetComponent<F3DLightning>();
        //beam2setting = beam2.GetComponent<F3DLightning>();
        //beam1setting.beamScale = 0.1f;
        //beam1.transform.localScale = new Vector3(1, 1, range/10);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
