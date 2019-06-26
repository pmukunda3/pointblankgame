using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    public GameObject Impact;
    public float velocity = 100f;
    public float range = 50f;
    public float impactOffset;

    private GameObject NewImpact;
    private RaycastHit hit;
    private float timeout;

    public bool dontDestroy
    {
        get;
        set;
    }
    private float clock = 0f;

    void Start()
    {
        timeout = range / velocity;
    }

    // Update is called once per frame
    void Update()
    {
        if (!dontDestroy)
        {
            clock += Time.deltaTime;
            if (clock > timeout)
            {
                Destroy(gameObject);
            }
            if (Physics.Raycast(transform.position,transform.forward, out hit,velocity*Time.deltaTime))
            {
                OnHit(hit);
            }
            transform.Translate(0, 0, velocity * Time.deltaTime);
        }
    }

    void OnHit(RaycastHit hit)
    {
        if (!dontDestroy)
        {
            Vector3 pos = hit.point + hit.normal * impactOffset;
            Quaternion rot = Quaternion.FromToRotation(Vector3.up, hit.normal);
            NewImpact = Instantiate(Impact, pos, rot);
            NewImpact.transform.parent = null;
            NewImpact.SetActive(true);
            Destroy(gameObject);
        }
    }

    /*
    void OnCollisionEnter(Collision collision)
    {
        if (!dontDestroy)
        {
            velocity = 0;
            ContactPoint contact = collision.GetContact(collision.contactCount-1);
            Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
            Vector3 pos = contact.point + contact.normal*impactOffset;
            NewImpact = Instantiate(Impact,pos,rot);
            NewImpact.transform.parent = gameObject.transform;
            //NewImpact.transform.Translate(0, 0, -impactOffset, Space.Self);
            NewImpact.transform.parent = null;
            NewImpact.SetActive(true);
            Destroy(gameObject);
        }
            
    }
    */
}