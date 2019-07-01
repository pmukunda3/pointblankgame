using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInputController : MonoBehaviour
{
    public bool ApplyRootMotion;
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void OnAnimatorMove()
    {
        if (ApplyRootMotion)
        { transform.position += anim.deltaPosition; }
        transform.parent.position = transform.position;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(Vector3.zero);

    }

}
