using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletWeapon : MonoBehaviour, IWeaponFire
{
    public float muzzleDuration, fireRate, minEjectAngle, maxEjectAngle, minEjectVel, maxEjectVel;
    public AudioClip gunshotSound;

    private GameObject muzzle, projectile, newProjectile, cartridge, newCartridge;
    private AudioSource audioSource;
    private Rigidbody rb;
    private float clock, fireInterval;
    

    // Start is called before the first frame update
    void Start()
    {
        clock = 0f;
        fireInterval = 1 / fireRate;
        muzzle = transform.Find("Muzzle").gameObject;
        muzzle.SetActive(false);
        projectile = transform.Find("Projectile").gameObject;
        cartridge = transform.Find("Cartridge").gameObject;
        audioSource = GetComponentInParent<AudioSource>();
    }

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
            audioSource.PlayOneShot(gunshotSound,1f);
            muzzle.SetActive(true);

            newProjectile = Instantiate(projectile, muzzle.transform);
            newProjectile.transform.parent = null;
            newProjectile.SetActive(true);

            newCartridge = Instantiate(cartridge, transform);
            newCartridge.SetActive(true);
            rb = newCartridge.GetComponent<Rigidbody>();
            rb.AddRelativeForce(Random.Range(minEjectVel, maxEjectVel) * (Quaternion.Euler(0, 0, Random.Range(minEjectAngle, maxEjectAngle)) * Vector3.right));
            newCartridge.transform.parent = null;
       
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
