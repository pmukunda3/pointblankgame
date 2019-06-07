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

    public float correctiveForce;
    public float dampeningForce;

    private PrototypePlayerController playerController;

    void Start() {
        camera = gameObject.GetComponent<Camera>();
        playerController = player.GetComponent<PrototypePlayerController>();
    }

    public Vector3 cameraCorrectionVelocity = new Vector3(0f, 0f, 0f);
    public Vector3 previousPosition;

    void LateUpdate() {
        Vector3 desiredLocation = player.transform.position + player.transform.rotation * (localPositionPivot + offset);
        Vector3 direction = transform.position - desiredLocation;

        Vector3 test = Quaternion.Inverse(player.transform.rotation) * direction;
        Debug.Log(test.z);

        if (test.z > maxIn) {
            transform.position += player.transform.rotation * ((test.z - maxIn) * -Vector3.forward);
            direction = transform.position - desiredLocation;
        }
        else if (test.z < -maxOut) {
            transform.position += player.transform.rotation * ((test.z - (-maxOut)) * -Vector3.forward);
            direction = transform.position - desiredLocation;
        }

        Vector3 cameraMovementCorrection;
        // F = ma = m * v/t = -k * x^2 - b * v, but only apply this along the camera's forward vector.
        if (Time.deltaTime > 0)
            cameraMovementCorrection = -correctiveForce * direction + dampeningForce * (previousPosition - transform.position) / Time.deltaTime;
        else
            cameraMovementCorrection = -correctiveForce * direction;
        Vector3 springForce = -correctiveForce * Mathf.Pow(direction.magnitude, 2) * direction;

        Vector3 damperVec = Vector3.zero;
        if (Time.deltaTime > 0)
            damperVec = dampeningForce * (previousPosition - transform.position) / Time.deltaTime;

        Debug.Log(previousPosition - transform.position);
        Debug.Log("Spring = (" + springForce.x + ", " + springForce.y + ", " + springForce.z + ") - Damper = (" + damperVec.x + ", " + damperVec.y + ", " + damperVec.z + ")");

        cameraCorrectionVelocity += cameraMovementCorrection * Time.deltaTime;

        previousPosition = transform.position;

        transform.position += cameraMovementCorrection * Time.deltaTime * Time.deltaTime; // cameraCorrectionVelocity * Time.deltaTime;
        transform.rotation = playerController.AimDirection() * Quaternion.Euler(12f, 0f, 0f);

        Vector3 cameraAlignProjection = Vector3.Project(transform.position - desiredLocation, playerController.AimDirection() * -Vector3.forward);
        transform.position = desiredLocation + cameraAlignProjection;
    }
}
