using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Landmine : MonoBehaviour
{
    public float delay;
    public float triggerRadius;
    public GameObject explosionEffect;
    public float blastRadius;
    public float blastForce;

    private BoxCollider objCollider; 
    private SphereCollider trigger;
    private bool plopped;
    private bool armed;
    private GameObject newExplosion;
    private float clock;

    void Plop(Vector3 point, Vector3 normal)
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        objCollider.enabled = false;
        transform.position = point;
        transform.rotation = Quaternion.FromToRotation(Vector3.up,normal);
        plopped = true;
    }

    void Arm()
    {
        trigger.enabled = true;
        armed = true;
    }

    void Explode()
    {
        Vector3 explosionPos = transform.position;
        Collider[] colliders = Physics.OverlapSphere(explosionPos, blastRadius);
        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null)
                rb.AddExplosionForce(blastForce, explosionPos, blastRadius, 0F);
        }
        newExplosion = Instantiate(explosionEffect, transform);
        newExplosion.SetActive(true);
        newExplosion.transform.parent = null;
        Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        clock = 0f;
        objCollider = GetComponent<BoxCollider>();
        objCollider.enabled = true;
        trigger = GetComponent<SphereCollider>();
        trigger.radius = triggerRadius;
        trigger.enabled = false;
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
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Explode();
        }
    }
}
