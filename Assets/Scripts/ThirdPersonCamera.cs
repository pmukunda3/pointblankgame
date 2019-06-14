using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour {

    private Camera camera;
    public GameObject player;

    public Vector3 offset;
    public GameObject cameraPivot;

    public AnimationCurve pitchOffsetFuncX;
    public AnimationCurve pitchOffsetFuncY;
    public AnimationCurve pitchOffsetFuncZ;

    //public float maxIn;
    //public float maxOut;

    //public float cameraMovementScalar;

    private IPlayerAim playerController;
    private Vector3 previousPosition;
    private Quaternion previousRotation;

    void Start() {
        camera = gameObject.GetComponent<Camera>();
        playerController = player.GetComponent<IPlayerAim>();

        previousPosition = transform.position;
        previousRotation = playerController.AimDirection();
    }

    void LateUpdate() {
        Vector3 pitchAdjustedOffset = new Vector3(
            pitchOffsetFuncX.Evaluate(playerController.AimPitch() / 90.0f) * offset.x,
            pitchOffsetFuncY.Evaluate(playerController.AimPitch() / 90.0f) * offset.y,
            pitchOffsetFuncZ.Evaluate(playerController.AimPitch() / 90.0f) * offset.z);
        Vector3 desiredLocation = cameraPivot.transform.position + playerController.AimDirection() * pitchAdjustedOffset;

        //Vector3 cameraAlignProjection = Vector3.Project(transform.position - desiredLocation, playerController.AimDirection() * -Vector3.forward);

        previousPosition = transform.position;
        previousRotation = transform.rotation;

        transform.rotation = playerController.AimDirection() * Quaternion.Euler(2f, 0f, 0f);
        transform.position = desiredLocation; // + cameraAlignProjection;
    }
}
