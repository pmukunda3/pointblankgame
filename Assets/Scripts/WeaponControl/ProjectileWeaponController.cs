﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileWeaponController : MonoBehaviour
{
    public GameObject Muzzle;
    public GameObject Projectile;
    public float MuzzleDuration;
    public float FireRate;

    private float clock;
    private float FireInterval;
    private GameObject NewProjectile;

    // Start is called before the first frame update
    void Start()
    {
        clock = 0f;
        FireInterval = 1 / FireRate;
        Muzzle.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButton("Fire1"))
        {
            if (clock >= 0f)
            {
                Muzzle.SetActive(true);
                clock = -FireInterval;
                NewProjectile = Instantiate(Projectile, Muzzle.transform);
                NewProjectile.transform.parent = null;
                NewProjectile.SetActive(true);
            }
        }
        if (clock < 0f)
        {
            clock += Time.deltaTime;
            if (clock >= -FireInterval + MuzzleDuration)
            {
                Muzzle.SetActive(false);
            }
        }
    }
}
