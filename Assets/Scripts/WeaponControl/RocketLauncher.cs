using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketLauncher : MonoBehaviour, IWeaponFire
{
    public MeshRenderer rocketMesh;
    public GameObject rocket;
    public Transform firePoint;
    public float fireRate;
    public float heatPerShot;
    public AudioClip fireSound;

    private float clock;
    private float fireInterval;
    private GameObject newRocket;
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        clock = 0f;
        fireInterval = 1 / fireRate;
        audioSource = GetComponentInParent<AudioSource>();
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

    public float GetHeatPerShot()
    {
        return heatPerShot;
    }

    public void FireWeapon()
    {
        // do nothing
    }

    public void FireWeaponDown()
    {
        if (clock >= 0f)
        {
            clock = -fireInterval;
            audioSource.PlayOneShot(fireSound);
            rocketMesh.enabled = false;
            newRocket = Instantiate(rocket, firePoint.transform);
            newRocket.transform.parent = null;
            newRocket.SetActive(true);
            EventManager.TriggerEvent<WeaponHeatEvent, float>(heatPerShot);
        }
    }

    public void FireWeaponUp()
    {
        // do nothing;
    }
}
