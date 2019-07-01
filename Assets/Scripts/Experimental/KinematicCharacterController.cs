﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KinematicCharacterController : MonoBehaviour, IPlayerAim {
    
    public float mouseSensitivity = 1.0f;
    public float screenMouseRatio = 1.777f;
    public Vector3 charVelocityRun;

    private new Rigidbody rigidbody;

    // Start is called before the first frame update
    void Start() {
        rigidbody = this.GetComponent<Rigidbody>();
    }

    private bool jump = false;

    private float aimPitch = 0f;
    private float mouseX;
    private float mouseY;
    private float moveZ;
    private float moveX;

    void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            if (!jump) {
                ctrlRbJump = jump = true;
            }
        }

        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");

        aimPitch += mouseSensitivity * mouseY * Time.deltaTime;
        if (aimPitch > 90f) {
            aimPitch = 90f;
        }
        else if (aimPitch < -90f) {
            aimPitch = -90f;
        }

        moveZ = Input.GetAxis("Vertical");
        moveX = Input.GetAxis("Horizontal");
    }

    private bool ctrlRbJump = false;

    void FixedUpdate() {
        rigidbody.MoveRotation(Quaternion.AngleAxis(screenMouseRatio * mouseSensitivity * mouseX * Time.deltaTime, Vector3.up) * rigidbody.rotation);
        rigidbody.MovePosition(transform.position + (transform.forward * moveZ * charVelocityRun.z + transform.right * moveX * charVelocityRun.x) * Time.deltaTime);
    }

    public Quaternion AimDirection() {
        return Quaternion.Euler(-aimPitch, rigidbody.rotation.y, 0f);
    }

    public float AimPitch() {
        return aimPitch;
    }

    public float AimYaw() {
        return rigidbody.rotation.y;
    }

    public Quaternion AimYawQuaternion() {
        return Quaternion.Euler(0f, rigidbody.rotation.y, 0f);
    }

    void OnCollisionEnter(Collision col) {
        ContactPoint contact = col.contacts[0];
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

        cube.transform.position = contact.point;
        cube.transform.localScale = Vector3.one * 0.1f;
    }
}
