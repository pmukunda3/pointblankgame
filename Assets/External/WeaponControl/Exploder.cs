using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public struct Explosion
{
    public float force;
    public Vector3 point;
    public float radius;
    public float yOffset;

    public Explosion(float force, Vector3 point, float radius, float yOffset)
    {
        this.force = force;
        this.point = point;
        this.radius = radius;
        this.yOffset = yOffset;
    }
}

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
            Vector3 blastPoint = transform.position;
            Explosion explosion = new Explosion(blastForce, blastPoint, blastRadius, 1f);
            Collider[] colliders = Physics.OverlapSphere(blastPoint, blastRadius, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);
            foreach (Collider hit in colliders)
            {
                Exploder ex = hit.GetComponent<Exploder>();
                if (ex != null)
                {
                    ex.Explode();
                    continue;
                }
                if (hit.gameObject.layer == LayerMask.NameToLayer("Player Character"))
                {
                    EventManager.TriggerEvent<PlayerControl.PlayerDamageEvent,int>(100);
                    continue;
                }
                if (hit.gameObject.CompareTag("AI"))
                {
                    EventManager.TriggerEvent<ExplosionDeathEvent, GameObject, Explosion>(hit.gameObject, explosion);
                    continue;
                }
                Rigidbody rb = hit.GetComponentInParent<Rigidbody>();
                if (rb != null)
                {
                    rb.AddExplosionForce(blastForce, blastPoint, blastRadius, 1F);
                }
            }
            newExplosion = Instantiate(explosionEffect, transform);
            newExplosion.SetActive(true);
            newExplosion.transform.parent = null;
            Destroy(gameObject);
        }
    }
}
