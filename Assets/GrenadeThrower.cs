using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeThrower : MonoBehaviour
{
    public GameObject grenade;
    public float throwVelocity;
    private GameObject newGrenade;

    private Rigidbody rb;

    void Throw()
    {
        newGrenade = Instantiate(grenade, transform);
        newGrenade.transform.parent = null;
        newGrenade.GetComponent<Grenade>().explode = true;
        newGrenade.GetComponent<Collider>().enabled = true;
        rb = newGrenade.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.AddRelativeForce(new Vector3(0,0,throwVelocity),ForceMode.VelocityChange);       
    }

    // Start is called before the first frame update
    void Start()
    {
        grenade.GetComponent<Collider>().enabled = false;
        grenade.GetComponent<Rigidbody>().isKinematic = true;
        grenade.GetComponent<Grenade>().explode = false;
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
