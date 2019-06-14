using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour {

    public GameObject player;
    public GameObject weaponPivot;

    public AnimationCurve offsetFuncY;
    public AnimationCurve offsetFuncZ;

    private Vector3 offset;

    private IPlayerAim playerController;
    private Vector3 previousPosition;
    private Quaternion previousRotation;

    void Start() {
        playerController = player.GetComponent<IPlayerAim>();

        previousPosition = transform.position;
        previousRotation = playerController.AimDirection();

        offset = transform.localPosition - weaponPivot.transform.localPosition; // or, transform.position - weaponPivot.transform.position;
    }

    void Update() {
        Vector3 pitchAdjustedOffset = new Vector3(offset.x, offsetFuncY.Evaluate(playerController.AimPitch() / 90.0f) * offset.y, offsetFuncZ.Evaluate(playerController.AimPitch() / 90.0f) * offset.z);
        Vector3 desiredLocation = weaponPivot.transform.position + playerController.AimDirection() * pitchAdjustedOffset;

        previousPosition = transform.position;
        previousRotation = transform.rotation;

        transform.rotation = playerController.AimDirection();
        transform.position = desiredLocation;

        if (Input.GetKeyDown(KeyCode.Mouse0)) { // Left Click
            EventManager.TriggerEvent<WeaponFirePrimary>();
        }

        if (Input.GetKeyDown(KeyCode.Mouse1)) { // Right Click
            EventManager.TriggerEvent<WeaponFireSecondary, float>(Random.value);
        }

        if (Input.GetKeyDown(KeyCode.Mouse2)) { // Scroll Click
            EventManager.TriggerEvent<WeaponFirePrimary>();
            EventManager.TriggerEvent<WeaponFireSecondary, float>(Random.value);
        }
    }
}
