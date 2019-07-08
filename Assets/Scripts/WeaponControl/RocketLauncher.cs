using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketLauncher : MonoBehaviour, IWeaponFire
{
    public MeshRenderer rocketMesh;
    public GameObject rocket;
    public Transform FirePoint;
    public float FireRate;

    private float clock;
    private float FireInterval;
    private GameObject NewRocket;

    // Start is called before the first frame update
    void Start()
    {
        clock = 0f;
        FireInterval = 1 / FireRate;
    }

    // Update is called once per frame
    void Update()
    {
        if (clock >= 0f)
        {
            rocketMesh.enabled = true;
        }
        else
        {
            clock += Time.deltaTime;
        }
    }

    public void FireWeapon()
    {
        // do nothing
    }

    public void FireWeaponDown()
    {
        if (clock >= 0f)
        {
            clock = -FireInterval;
            rocketMesh.enabled = false;
            NewRocket = Instantiate(rocket, FirePoint.transform);
            NewRocket.transform.parent = null;
            NewRocket.SetActive(true);
        }
    }

    public void FireWeaponUp()
    {
        // do nothing;
    }
}
