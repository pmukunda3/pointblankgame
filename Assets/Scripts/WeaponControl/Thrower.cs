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
    public Animator animator;
    public AudioClip throwSound;

    private GameObject newObject;
    private AudioSource audioSource;
    private Rigidbody rb;
    private int i, n;
    private Vector3 IKTargetPosition;
    private Quaternion IKTargetRotation;
    private float IKWeight;

    void Start()
    {
        //EventManager.StartListening<ThrowStartEvent>(new UnityAction(ThrowStart));
        EventManager.StartListening<ThrowReleaseEvent>(new UnityAction(ThrowRelease));
        audioSource = gameObject.GetComponent<AudioSource>();
        i = 0;
        n = transform.childCount;
        activeItem = transform.GetChild(i).gameObject;
    }

    void ThrowRelease()
    {
        audioSource.PlayOneShot(throwSound);
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
        // do nothing
    }
}
