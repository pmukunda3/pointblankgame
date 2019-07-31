using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.AI;
using System.Collections.Generic;

public class RobotEventManager : MonoBehaviour
{
    public Animator ai_animator;
    private NavMeshAgent nav_agent;
    public float health = 100f;
    public float maxHealth = 100f;
    private float adjustment = 2.3f;
    private Vector3 worldPosition;
    private Vector3 screenPosition;
    private Transform myTransform;
    private Camera myCamera;
    private int healthBarHeight = 5;
    private int healthBarLeft = 110;
    private int barTop = 1;


    // Use this for initialization
    void OnEnable()
    {
        myCamera = Camera.main;
        EventManager.StartListening<HitEnemyEvent, GameObject, float, GameObject>
        (new UnityEngine.Events.UnityAction<GameObject, float, GameObject>(GotHit));
       EventManager.StartListening<ExplosionDeathEvent, GameObject, Explosion>(new UnityEngine.Events.UnityAction<GameObject, Explosion>(ExplosionDeath));
    }
    void OnDisable()
    {
        EventManager.StopListening<HitEnemyEvent, GameObject, float, GameObject>
        (new UnityEngine.Events.UnityAction<GameObject, float, GameObject>
            (GotHit));

    }

    // Start is called before the first frame update
    void Start()
    {
        myTransform = gameObject.GetComponent<Collider>().transform;
        SetRagdoll(false);
        nav_agent = gameObject.GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        myTransform = gameObject.GetComponent<Collider>().transform;
        worldPosition = new Vector3(myTransform.position.x, myTransform.position.y + adjustment, myTransform.position.z);
        screenPosition = myCamera.WorldToScreenPoint(worldPosition);
    }


    private void OnGUI()
    {
        RaycastHit hit;
        if (Physics.Raycast(myTransform.position, myTransform.forward, out hit) && (health != maxHealth) && (health > 0))
        {
            GUI.color = Color.red;
            GUI.HorizontalScrollbar(new Rect(screenPosition.x - healthBarLeft / 2, Screen.height - screenPosition.y - barTop, 100, 0), 0, health, 0, maxHealth); //displays a healthbar
            GUI.color = Color.white;
            GUI.contentColor = Color.white;
            GUI.Label(new Rect(screenPosition.x - healthBarLeft / 2, Screen.height - screenPosition.y - barTop + 5, 100, 100), "" + health + "/" + maxHealth); //displays health in text format
        }

    }

    void SetKinematic(bool newValue)
    {
        Rigidbody[] bodies = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in bodies)
        {
            rb.isKinematic = newValue;
        }
    }

    void SetRagdoll(bool newValue)
    {
    //    gameObject.GetComponent<Collider>().enabled = !newValue;
    //    Transform root = transform.Find("Root");
    //    Collider[] colliders = root.GetComponentsInChildren<Collider>();
    //    foreach (Collider c in colliders)
    //    {
    //        c.enabled = newValue;
    //    }
    //    Rigidbody[] bodies = root.GetComponentsInChildren<Rigidbody>();
    //    foreach (Rigidbody rb in bodies)
    //    {
    //        rb.isKinematic = !newValue;
    //    }
    }

    private void ApplyForce(Vector3 force)
    {
        Transform root = transform.Find("Root");
        Rigidbody[] bodies = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in bodies)
        {
            rb.AddForce(force);
        }
    }


    private void ApplyExplosionForce(Explosion exp)
    {
        Transform root = transform.Find("Root");
        Rigidbody[] bodies = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in bodies)
        {
            rb.AddExplosionForce(exp.force, exp.point, exp.radius, exp.yOffset);
        }
    }

    private void ExplosionDeath(GameObject obj, Explosion exp)
    {
        if (obj == gameObject)
        {
            Debug.Log("Explo hit");
            health = 0;
            //SetRagdoll(true);
            //ai_animator.enabled = false;
            ai_animator.SetTrigger("Death");
            nav_agent.enabled = false;
            Rigidbody rb = GetComponent<Rigidbody>();
            //rb.AddForce(impact.transform.position);
            rb.AddExplosionForce(exp.force, exp.point, exp.radius, exp.yOffset);
            //ApplyExplosionForce(exp);
            Destroy(gameObject, 5);
        }
    }

    // Update is called once per frame
    private void GotHit(GameObject hit_obj, float hit_point, GameObject impact)
    {
    Debug.Log("Getting hit");
    Debug.Log(hit_obj);
    Debug.Log(gameObject.GetComponent<Collider>());
    if (hit_obj == gameObject)
    {
        health -= hit_point;
        if (health > 0)
        {

        }
        else
        {

            // disable nav
            gameObject.GetComponent<NavMeshAgent>().enabled = false;

            // trigger animation
            ai_animator.SetTrigger("Death");

            // enable ragdoll
            //SetKinematic(false);


            // add force
            //Rigidbody rb = GetComponent<Rigidbody>();
            //rb.AddForce(impact.transform.position);
            //rb.AddTorque(impact.transform.rotation);


            Destroy(gameObject, 5);
        }

    }
    }
}
