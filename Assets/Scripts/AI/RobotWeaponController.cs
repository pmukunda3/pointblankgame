using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotWeaponController : MonoBehaviour
{
    public float muzzleDuration, fireRate;
    public AudioClip gunshotSound;

    private GameObject muzzle, projectile, newProjectile;
    private float clock, fireInterval;
    //private UserInput userInput;
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        muzzle = transform.Find("Muzzle").gameObject;
        projectile = transform.Find("Projectile").gameObject;
        clock = 0f;
        fireInterval = 1 / fireRate;
        muzzle.SetActive(false);
        //userInput = gameObject.GetComponentInParent<UserInput>();
        audioSource = GetComponentInParent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (clock < 0f)
        {
            clock += Time.deltaTime;
            if (clock >= -fireInterval + muzzleDuration)
            {
                muzzle.SetActive(false);
            }
        }
    }

    public void FireWeapon()
    {
        if (clock >= 0f)
        {
            clock = -fireInterval;
            audioSource.PlayOneShot(gunshotSound, 1f);
            muzzle.SetActive(true);
            newProjectile = Instantiate(projectile, muzzle.transform);
            newProjectile.transform.parent = null;
            newProjectile.SetActive(true);
        }
    }

    public void FireWeaponDown()
    {
        // do nothing
    }

    public void FireWeaponUp()
    {
        // do nothing
    }
}
