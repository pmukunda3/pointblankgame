using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class Thrower : MonoBehaviour
{
    public float throwVelocity;
    public float throwInterval;
    public GameObject activeItem
    {
        get;
        private set;
    }
    public Animator animator;
    public AudioClip throwSound;
    public UserInput userInput;
    public GrenadeUI grenadeUI;

    private string UIMessage;
    private string throwKey;
    private GameObject newObject;
    private AudioSource audioSource;
    private Rigidbody rb;
    private int i, n;
    private Vector3 IKTargetPosition;
    private Quaternion IKTargetRotation;
    private float clock;

    void Start()
    {
        clock = 0f;
        throwKey = userInput.throwItemKey.ToString();
        EventManager.StartListening<ThrowReleaseEvent>(new UnityAction(ThrowRelease));
        audioSource = gameObject.GetComponent<AudioSource>();
        i = 0;
        n = transform.childCount;
        activeItem = transform.GetChild(i).gameObject;
    }

    void Update()
    {
        if (clock <= 0f)
        {
            UIMessage = string.Format("Press {0} to throw", throwKey);
        }
        else
        {
            clock -= Time.deltaTime;
            int seconds = (int)clock;
            UIMessage = string.Format("Next throw in {0}", seconds);
        }
        grenadeUI.DisplayMessage(UIMessage);
    }

    void ThrowRelease()
    {
        if(clock <= 0f)
        {
            clock = throwInterval;
            audioSource.PlayOneShot(throwSound);
            newObject = Instantiate(activeItem, transform);
            newObject.transform.parent = null;
            newObject.SetActive(true);
            rb = newObject.GetComponentInParent<Rigidbody>();
            rb.AddRelativeForce(new Vector3(0, 0, throwVelocity), ForceMode.VelocityChange);
        }        
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
