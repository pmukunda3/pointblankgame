using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContinuousWeaponController : MonoBehaviour
{
    public GameObject beam;

    private UserInput userInput;

    void Start()
    {
        userInput = gameObject.GetComponentInParent<UserInput>();
        beam.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(userInput.actions.primaryFire.down)
        {
            beam.SetActive(true);
        }
        if(userInput.actions.primaryFire.up)
        {
            beam.SetActive(false);
        }
    }
}
