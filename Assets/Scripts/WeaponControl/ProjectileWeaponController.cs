using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileWeaponController : MonoBehaviour
{
    public GameObject Muzzle;
    public GameObject Projectile;
    public GameObject FirePoint;
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
        Projectile.SetActive(false);
        Projectile.GetComponent<ProjectileController>().dontDestroy = true; 
    }

    // Update is called once per frame
    void Update()
    {
        clock += Time.deltaTime;
        if (clock > MuzzleDuration)
        {
            Muzzle.SetActive(false);
        }
        if (clock > FireInterval)
        {
            Muzzle.SetActive(true);
            clock = 0f;
            NewProjectile = Instantiate(Projectile, FirePoint.transform);
            NewProjectile.transform.parent = null;
            NewProjectile.SetActive(true);
            NewProjectile.GetComponent<ProjectileController>().dontDestroy = false;
        }
    }
}
