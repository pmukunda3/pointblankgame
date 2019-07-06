using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    public bool explode
    {
        get;
        set;
    }
    public float timeout;
    public GameObject explosionEffect;
    public float blastRadius;
    public float blastForce;

    private GameObject newExplosion;
    private float clock;

    // Start is called before the first frame update
    void Start()
    {
        clock = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (explode)
        {
            clock += Time.deltaTime;
            if (clock >= timeout)
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
        }
    }
}
