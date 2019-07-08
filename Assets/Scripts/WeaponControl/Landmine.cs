using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Landmine : MonoBehaviour
{
    public float delay;
    public Vector3 triggerSize;
    //public Collider trigger;

    private BoxCollider col; 
    private bool plopped;
    private bool armed;
    private float clock;

    private float Circumradius(float r, float h)
    { 
        return ((r*r)+(h*h))/(2*h);
    }

    void Plop(Vector3 point, Vector3 normal)
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        col.enabled = false;
        transform.position = point;
        transform.rotation = Quaternion.FromToRotation(Vector3.up,normal);
        clock = 0f;
        plopped = true;       
    }

    void Arm()
    {
        col.size = triggerSize;
        col.isTrigger = true;
        col.enabled = true;
        armed = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        col = GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        if(plopped && !armed)
        {
            clock += Time.deltaTime;
            if(clock >= delay)
            {
                Arm();
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if(!plopped)
        {
            ContactPoint contact = collision.GetContact(0);
            Plop(contact.point, contact.normal);            
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(armed)
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                gameObject.GetComponent<Exploder>().Explode();
            }
        }        
    }
}
