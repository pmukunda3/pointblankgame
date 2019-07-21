using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.AI;
using System.Collections.Generic;

public class RobotEventManager : MonoBehaviour
{
    public Animator ai_animator;
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
        if (Physics.Raycast(myTransform.position, myTransform.forward, out hit) &&
        health != maxHealth)
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
                Rigidbody rb = GetComponent<Rigidbody>(); ;
                rb.AddForce(impact.transform.position);
                //rb.AddTorque(impact.transform.rotation);


                Destroy(gameObject, 5);
            }

        }
    }

}
