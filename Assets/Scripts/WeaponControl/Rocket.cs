using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    public float velocity = 100f;
    public float range = 100f;
    public float impactOffset = 0.2f;
    public AudioSource launcherAudio;

    private Exploder exploder;
    private RaycastHit hit;
    private float timeout;
    private float clock;

    void Start()
    {
        clock = 0f;
        exploder = gameObject.GetComponent<Exploder>();
        timeout = range / velocity;
    }

    // Update is called once per frame
    void Update()
    {
        clock += Time.deltaTime;
        if (clock > timeout)
        {
            launcherAudio.Stop();
            exploder.Explode();
        }
        if (Physics.Raycast(transform.position, transform.forward, out hit, velocity * Time.deltaTime, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
        {
            transform.position = hit.point - impactOffset * transform.forward;
            launcherAudio.Stop();
            exploder.Explode();
        }
        transform.Translate(0, 0, velocity * Time.deltaTime);
    }
}
