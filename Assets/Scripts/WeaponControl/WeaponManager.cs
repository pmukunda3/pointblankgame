using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    private int i, n;
    public GameObject activeWeapon
    {
        get;
        private set;
    }

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
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Change Weapon"))
        {
            activeWeapon.SetActive(false);
            i = ++i % n;
            activeWeapon = transform.GetChild(i).gameObject;
            activeWeapon.SetActive(true);
        }
    }
}
