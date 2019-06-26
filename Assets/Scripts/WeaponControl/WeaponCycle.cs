using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCycle : MonoBehaviour
{
    private int n;
    private int i;

    // Start is called before the first frame update
    void Start()
    {
        n = transform.childCount;
        for (i = 0; i < n; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
        i = 0;
        transform.GetChild(i).gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Change Weapon"))
        {
            transform.GetChild(i).gameObject.SetActive(false);
            i = ++i % n;
            transform.GetChild(i).gameObject.SetActive(true);
        }
    }
}
