using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Animator))]
public class PrototypePlayerController : MonoBehaviour {

    private Rigidbody rigidbody;
    private Animator animator;

    void Start() {
        rigidbody = gameObject.GetComponent<Rigidbody>();
        animator = gameObject.GetComponent<Animator>();

        //animator.SetFloat("velLocalZ", 0.2f);
        animator.SetBool("idle", false);
        animator.SetInteger("moveMode", 2);
    }

    void Update() {
        
        if (Input.GetKey(KeyCode.W)) {

        }

        if (Input.GetKey(KeyCode.A)) {

        }

        if (Input.GetKey(KeyCode.S)) {

        }

        if (Input.GetKey(KeyCode.D)) {

        }
    }

    void FixedUpdate() {
        //rigidbody.transform.position += transform.rotation * (0.5f * Vector3.forward * Time.deltaTime);
        rigidbody.velocity = 0.5f * transform.forward;
    }
}
