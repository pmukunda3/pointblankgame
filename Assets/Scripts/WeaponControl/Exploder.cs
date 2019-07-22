using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class Exploder : MonoBehaviour
{
    public GameObject explosionEffect;
    public float blastRadius, blastForce;
    public AudioClip explosionSound;
    //public float soundRadius;

    private bool exploded;
    private GameObject newExplosion;

    private void MakeExplosionSound()
    {
        GameObject emptyGameObject = new GameObject();
        AudioSource newAudioSource = emptyGameObject.AddComponent<AudioSource>();
        newAudioSource.volume = 1f;
        newAudioSource.spatialBlend = 0f;
        //newAudioSource.spread = 45f;
        //newAudioSource.minDistance = soundRadius;
        newAudioSource.PlayOneShot(explosionSound);
        Destroy(emptyGameObject, explosionSound.length + 1f);
    }

    public void Explode()
    {
        if (!exploded)
        {
            exploded = true;
            MakeExplosionSound();
            Vector3 explosionPos = transform.position;
            Collider[] colliders = Physics.OverlapSphere(explosionPos, blastRadius, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);
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

                    {
                        if (rb.gameObject.CompareTag("AI"))
                        {
                            EventManager.TriggerEvent<RagdollEvent, GameObject>(rb.gameObject
                            );
                        }

                        rb.AddExplosionForce(blastForce, explosionPos, blastRadius, 0F);
                    }
                }
            }
            newExplosion = Instantiate(explosionEffect, transform);
            newExplosion.SetActive(true);
            newExplosion.transform.parent = null;
            Destroy(gameObject);
        }
    }
}
