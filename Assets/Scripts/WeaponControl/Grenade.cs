using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    public AudioClip collisionSound;
    public float timeout;

    private float clock;
    private bool soundPlayed;
    

    // Start is called before the first frame update
    void Start()
    {
        clock = 0f;
        soundPlayed = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!soundPlayed)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Static Level Geometry"))
            {
                gameObject.GetComponent<AudioSource>().PlayOneShot(collisionSound);
                soundPlayed = true;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        clock += Time.deltaTime;
        if (clock >= timeout)
        {
            gameObject.GetComponent<Exploder>().Explode();
        }
    }
}
