using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContinuousWeaponController : MonoBehaviour, IWeaponFire
{
    public GameObject beam;

    private UserInput userInput;

    void Start()
    {
        userInput = gameObject.GetComponentInParent<UserInput>();
        beam.SetActive(false);
    }

    public void FireWeapon() {
        // do nothing
    }

    public void FireWeaponDown() {
        beam.SetActive(true);
    }

    public void FireWeaponUp() {
        beam.SetActive(false);
    }
}
