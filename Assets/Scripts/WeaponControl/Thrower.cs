using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thrower : MonoBehaviour
{
    public GameObject throwObject;
    public float throwVelocity;
    private GameObject newObject;

    private Rigidbody rb;

    void Throw()
    {
        newObject = Instantiate(throwObject, transform);
        newObject.transform.parent = null;
        newObject.SetActive(true);        
        rb = newObject.GetComponentInParent<Rigidbody>();
        rb.AddRelativeForce(new Vector3(0,0,throwVelocity),ForceMode.VelocityChange);       
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Throw();
        }
    }
}
