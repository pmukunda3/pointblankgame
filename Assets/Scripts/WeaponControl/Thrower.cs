using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class Thrower : MonoBehaviour
{
    public float throwVelocity;
    public GameObject activeItem
    {
        get;
        private set;
    }

    private GameObject newObject;
    private Rigidbody rb;
    private int i, n;

    void Start()
    {
        EventManager.StartListening<ThrowEvent>(new UnityAction(Throw));
        i = 0;
        n = transform.childCount;
        activeItem = transform.GetChild(i).gameObject;
    }

    void Throw()
    {
        newObject = Instantiate(activeItem, transform);
        newObject.transform.parent = null;
        newObject.SetActive(true);
        rb = newObject.GetComponentInParent<Rigidbody>();
        rb.AddRelativeForce(new Vector3(0, 0, throwVelocity), ForceMode.VelocityChange);
    }

    public void ChangeItem()
    {
        i = ++i % n;
        activeItem = transform.GetChild(i).gameObject;
    }

    public void AnimatorIK()
    {
        
    }
}
