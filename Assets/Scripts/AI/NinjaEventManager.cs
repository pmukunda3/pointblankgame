using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.AI;
using System.Collections.Generic;


public class NinjaEventManager : MonoBehaviour
{
    public Animator ai_animator;
    private NavMeshAgent nav_agent;
    public float health = 100f;
    public float maxHealth = 100f;
    private float adjustment = 2.3f;
    private Vector3 worldPosition;
    private Vector3 screenPosition;
    private Transform myTransform ;
    private Camera myCamera ;
    private int healthBarHeight = 5;
    private int healthBarLeft = 110;
    private int barTop = 1;
   


    // Use this for initialization
    void OnEnable()
    {
        myCamera = Camera.main;
        EventManager.StartListening<HitEnemyEvent, GameObject, float, GameObject>
        (new UnityEngine.Events.UnityAction<GameObject, float, GameObject>(GotHit));
        //EventManager.StartListening<RagdollEvent, GameObject>(new UnityEngine.Events.UnityAction<GameObject>(EnableRagDoll));
        EventManager.StartListening<ExplosionDeathEvent, GameObject, Explosion>(new UnityEngine.Events.UnityAction<GameObject,Explosion>(ExplosionDeath));
    }
    void OnDisable()
    {
        EventManager.StopListening<HitEnemyEvent, GameObject, float, GameObject>
        (new UnityEngine.Events.UnityAction<GameObject, float, GameObject>
            (GotHit));

    }
    private void Start()
    {
        SetRagdoll(false);
        myTransform = gameObject.GetComponent<Collider>().transform;
        nav_agent = gameObject.GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        myTransform = gameObject.GetComponent<Collider>().transform;
        worldPosition = new Vector3(myTransform.position.x, myTransform.position.y + adjustment, myTransform.position.z);
        screenPosition = myCamera.WorldToScreenPoint(worldPosition);
    }

    private void OnGUI()
    {
        RaycastHit hit;
        if(Physics.Raycast(myTransform.position, myTransform.forward, out hit) && health < maxHealth && health > 0)
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
        gameObject.GetComponent<Collider>().enabled = !newValue;
        Transform root = transform.Find("Root");
        Collider[] colliders = root.GetComponentsInChildren<Collider>();
        foreach (Collider c in colliders)
        {
            c.enabled = newValue;
        }
        Rigidbody[] bodies = root.GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in bodies)
        {
            rb.isKinematic = !newValue;
        }
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
            rb.AddExplosionForce(exp.force,exp.point,exp.radius,exp.yOffset);
        }
    }

    private void EnableRagDoll(GameObject obj)
    {
        if(obj == gameObject)
        {
            Debug.Log("Disable animation");
            SetKinematic(false);
            ai_animator.enabled = false;
            nav_agent.enabled = false;
            Destroy(gameObject, 2);
        }
    }

    private void ExplosionDeath(GameObject obj, Explosion exp)
    {
        if (obj == gameObject)
        {
            //SetKinematic(false);
            SetRagdoll(true);
            ai_animator.enabled = false;
            nav_agent.enabled = false;
            ApplyExplosionForce(exp);
            Destroy(gameObject, 5);
        }
    }

    // Update is called once per frame
    private void GotHit(GameObject hit_obj,float hit_point, GameObject impact)
    {
        Debug.Log("Getting hit");
        Debug.Log(hit_obj);
        Debug.Log(gameObject.GetComponent<Collider>());
        if (hit_obj == gameObject)
        {
            health -= hit_point;

            if(health <= 0)
            {
                ai_animator.enabled = false;
                nav_agent.enabled = false;
                SetRagdoll(true);

                // To make enemy fall forward
                ApplyForce(25f * transform.forward);

                Destroy(gameObject, 2);
            }

        }

    }
}
