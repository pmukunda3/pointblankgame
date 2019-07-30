using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerControl;

public class RobotProjectileController : MonoBehaviour
{
    public GameObject Impact;
    public float velocity = 100f;
    public float range = 50f;
    public float impactOffset;
    public int damage = 10;
    private GameObject NewImpact;
    private RaycastHit hit;
    private float timeout;

    private float clock;

    void Start()
    {
        timeout = range / velocity;
        clock = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        clock += Time.deltaTime;
        if (clock > timeout)
        {
            Destroy(gameObject);
        }
        if (Physics.Raycast(transform.position, transform.forward, out hit, velocity * Time.deltaTime, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
        {
            OnHit(hit);
        }
        transform.Translate(0, 0, velocity * Time.deltaTime);
    }

    void OnHit(RaycastHit hit)
    {
        Exploder ex = hit.collider.gameObject.GetComponent<Exploder>();
        if (ex != null)
        {
            ex.Explode();
        }
        Vector3 pos = hit.point + hit.normal * impactOffset;
        Quaternion rot = Quaternion.FromToRotation(Vector3.forward, hit.normal);
        if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Static Level Geometry"))
        {
            NewImpact = Instantiate(Impact, pos, rot);
            NewImpact.transform.parent = null;
            NewImpact.SetActive(true);
        }
        Destroy(gameObject);
        EventManager.TriggerEvent<PlayerDamageEvent, int>(damage);

    }
}
