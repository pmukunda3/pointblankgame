using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exploder : MonoBehaviour
{

    public GameObject explosionEffect;
    public float blastRadius;
    public float blastForce;
    public bool exploded
    {
        get;
        private set;
    }

    private GameObject newExplosion;

    public void Explode()
    {
        if (!exploded)
        {
            exploded = true;
            Vector3 explosionPos = transform.position;
            Collider[] colliders = Physics.OverlapSphere(explosionPos, blastRadius);
            foreach (Collider hit in colliders)
            {
                Exploder ex = hit.GetComponent<Exploder>();
                if (ex != null)
                {
                    ex.Explode();
                }
                else
                {
                    Rigidbody rb = hit.GetComponentInParent<Rigidbody>();
                    if (rb != null)
                        rb.AddExplosionForce(blastForce, explosionPos, blastRadius, 0F);
                }
            }
            newExplosion = Instantiate(explosionEffect, transform);
            newExplosion.SetActive(true);
            newExplosion.transform.parent = null;
            Destroy(gameObject);
        }
    }
}
