using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour {

    private Camera camera;
    public GameObject player;

    public Vector3 offset;
    public Vector3 localPositionPivot;

    public float maxIn;
    public float maxOut;

    public float cameraMovementScalar;

    private PrototypePlayerController playerController;

    void Start() {
        camera = gameObject.GetComponent<Camera>();
        playerController = player.GetComponent<PrototypePlayerController>();

        previousPosition = transform.position;
        previousRotation = playerController.AimDirection();
    }

    public Vector3 previousPosition;
    public Quaternion previousRotation;
    public Quaternion relativeRotation;

    void LateUpdate() {
        Quaternion cameraRotation = playerController.AimDirection();
        Vector3 desiredLocation = player.transform.position + cameraRotation * offset;
        Vector3 cameraAlignProjection = Vector3.Project(transform.position - desiredLocation, playerController.AimDirection() * -Vector3.forward);

            // Doesn't work, I'll have to break apart cameraRotation to two separate quaternions.
        Vector3 cameraDirection = desiredLocation - cameraRotation * localPositionPivot;

        previousPosition = transform.position;
        previousRotation = transform.rotation;

        transform.rotation = playerController.AimDirection() * Quaternion.Euler(12f, 0f, 0f);
        transform.position = desiredLocation; // + cameraAlignProjection;
    }
}
