using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotWeaponCtrl : MonoBehaviour
{
    public GameObject player;
    public GameObject weaponPivot;

    public AnimationCurve offsetFuncY;
    public AnimationCurve offsetFuncZ;

    public Vector3 weaponDownPosition = new Vector3(-0.1f, -0.34f, 0.18f);

    public bool aimingWeapon { get; set; }

    private Vector3 offset;

    private IPlayerAim playerController;
    private Vector3 previousPosition;
    private Quaternion previousRotation;

    void Start()
    {
        playerController = player.GetComponent<IPlayerAim>();

        previousPosition = transform.position;
        previousRotation = playerController.AimDirection();

        offset = transform.localPosition - weaponPivot.transform.localPosition; // or, transform.position - weaponPivot.transform.position;
        aimingWeapon = false;
    }

    void Update()
    {
        Debug.Log(aimingWeapon);
        if (aimingWeapon)
        {
            Vector3 pitchAdjustedOffset = Vector3.Scale(offset,
                new Vector3(1f, offsetFuncY.Evaluate(playerController.AimPitch() / 90.0f), offsetFuncZ.Evaluate(playerController.AimPitch() / 90.0f)));
            Vector3 desiredLocation = weaponPivot.transform.position + playerController.AimDirection() * pitchAdjustedOffset;

            previousPosition = transform.position;
            previousRotation = transform.rotation;

            transform.rotation = playerController.AimDirection();
            transform.position = desiredLocation;

            if (Input.GetKeyDown(KeyCode.Mouse0))
            { // Left Click
                EventManager.TriggerEvent<WeaponFirePrimary>();
            }

            if (Input.GetKeyDown(KeyCode.Mouse1))
            { // Right Click
                EventManager.TriggerEvent<WeaponFireSecondary, float>(Random.value);
            }

            if (Input.GetKeyDown(KeyCode.Mouse2))
            { // Scroll Click
                EventManager.TriggerEvent<WeaponFirePrimary>();
                EventManager.TriggerEvent<WeaponFireSecondary, float>(Random.value);
            }
        }
        else
        {
             //transform.rotation = player.transform.rotation * Quaternion.Euler(0f, 90f, 0f) * Quaternion.Euler(35f, 0f, 0f);
            //transform.position = weaponPivot.transform.position + player.transform.rotation * weaponDownPosition;
        }
    }
}
