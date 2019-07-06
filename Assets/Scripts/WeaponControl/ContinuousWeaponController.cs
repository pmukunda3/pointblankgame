using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContinuousWeaponController : MonoBehaviour
{
    public GameObject beam;

    void OnEnable()
    {
        beam.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Fire1"))
        {
            beam.SetActive(true);
        }
        if(Input.GetButtonUp("Fire1"))
        {
            beam.SetActive(false);
        }
    }
}
