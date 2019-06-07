using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PrototypePlayerController : MonoBehaviour {

    private Rigidbody rigidbody;
    private CharacterController charController;

    void Start() {
        rigidbody = gameObject.GetComponent<Rigidbody>();
        charController = gameObject.GetComponent<CharacterController>();
    }

    void Update() {
        //rigidbody.transform.position += transform.rotation * (0.5f * Vector3.forward * Time.deltaTime);
        //rigidbody.velocity = 5f * transform.forward;
        charController.Move(transform.rotation * (Vector3.forward * 0.5f * Time.deltaTime));
    }
}
