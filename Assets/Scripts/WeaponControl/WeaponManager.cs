using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour, IWeaponFire
{
    private int i, n;
    public GameObject activeWeapon
    {
        get;
        private set;
    }

    private UserInput userInput;
    private IWeaponFire currentWeapon;

    // Start is called before the first frame update
    void Start()
    {
        n = transform.childCount;
        for (i = 0; i < n; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
        i = 0;
        activeWeapon = transform.GetChild(i).gameObject;
        activeWeapon.SetActive(true);
        currentWeapon = activeWeapon.GetComponent<IWeaponFire>() as IWeaponFire;

        userInput = gameObject.GetComponentInParent<UserInput>();
    }

    // Update is called once per frame
    public void ChangeWeapon()
    {
        activeWeapon.SetActive(false);
        i = ++i % n;
        activeWeapon = transform.GetChild(i).gameObject;
        activeWeapon.SetActive(true);
        currentWeapon = activeWeapon.GetComponent<IWeaponFire>() as IWeaponFire;
    }

    public void FireWeapon() {
        currentWeapon.FireWeapon();
    }

    public void FireWeaponDown() {
        currentWeapon.FireWeaponDown();
    }

    public void FireWeaponUp() {
        currentWeapon.FireWeaponUp();
    }
}
