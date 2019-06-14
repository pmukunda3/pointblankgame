using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct MovementCharacteristics {
    public AnimationCurve accelerationCurve;
    public AnimationCurve passiveDecelerationCurve;
    public AnimationCurve activeDecelerationCurve;
    public float maxSpeed;
    public float accelerationScalar;

    public float Acceleration(float velocity) {
        return accelerationScalar * accelerationCurve.Evaluate(velocity / maxSpeed);
    }

    public float Deceleration(float velocity, bool active = false) {
        if (active) return accelerationScalar * activeDecelerationCurve.Evaluate(1.0f - velocity / maxSpeed);
        else return accelerationScalar * passiveDecelerationCurve.Evaluate(1.0f - velocity / maxSpeed);
    }
}

public class PlayerController : MonoBehaviour, IPlayerAim {

    public enum MoveMode : int {
        Running = 0,
        Falling,
        Climbing,
        Landing
    };

    [System.Serializable]
    public struct CharacterMovementCharacteristics {
        public MovementCharacteristics forward;
        public MovementCharacteristics reverse;
        public MovementCharacteristics lateral;
    }

    public const float EPSILON = 1e-6f;

    public float mouseSensitivity = 1.0f;
    public float screenMouseRatio = 1.777f;

    public Vector2 deadzone = new Vector2(0.01f, 0.01f);

    public IMovementState runningState;

    public WeaponController weaponController;

    private Rigidbody rigidbody;
    private Animator animator;

    void Start() {
        rigidbody = gameObject.GetComponent<Rigidbody>();
        animator = gameObject.GetComponent<Animator>();

        runningState = gameObject.GetComponent<Running>() as IMovementState;

        currentMoveState = runningState;

        EventManager.StartListening<WeaponFirePrimary>(
            new UnityEngine.Events.UnityAction(WeaponFirePrimaryCallbackTest));
        EventManager.StartListening<WeaponFireSecondary, float>(
            new UnityEngine.Events.UnityAction<float>(WeaponFirePrimaryCallbackTest));
    }

    private IMovementState currentMoveState;
    private bool jump = false;
    private bool climbing = false;
    private bool climbingLowerTrigger = false;

    private float aimPitch = 0f;

    private float speedTargetX;
    private float speedTargetY;
    private float mouseX;
    private float mouseY;

    private Vector3 localVelocity = Vector3.zero;
    private Vector2 input = Vector2.zero;

    public Quaternion AimDirection() {
        return Quaternion.Euler(-aimPitch, transform.eulerAngles.y, 0f);
    }

    public float AimPitch() {
        return aimPitch;
    }

    private void WeaponFirePrimaryCallbackTest() {
        Debug.Log("Fire Weapon Primary");
    }

    private void WeaponFirePrimaryCallbackTest(float holdTime) {
        Debug.Log("Fire Weapon Secondary: " + holdTime);
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            if (!jump) {
                jump = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            transform.position = transform.position + new Vector3(0f, 20f, 0f);
        }

        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");

        aimPitch += mouseSensitivity * mouseY * Time.deltaTime;
        if (aimPitch > 80f) {
            aimPitch = 80f;
        }
        else if (aimPitch < -80f) {
            aimPitch = -80f;
        }

        // Temporary until I get the targetting code correct
        //speedTargetX = Input.GetAxis("Horizontal");
        //speedTargetY = Input.GetAxis("Vertical");

        speedTargetX = speedTargetY = 0f;
        if (Input.GetKey(KeyCode.W)) speedTargetY += 1.0f;
        if (Input.GetKey(KeyCode.A)) speedTargetX -= 1.0f;
        if (Input.GetKey(KeyCode.S)) speedTargetY -= 1.0f;
        if (Input.GetKey(KeyCode.D)) speedTargetX += 1.0f;

        if (Input.GetKey(KeyCode.LeftShift)) {
            if (speedTargetX > 0.5f) {
                speedTargetX = 0.5f;
            }
            else if (speedTargetX < -0.5f) {
                speedTargetX = -0.5f;
            }

            if (speedTargetY > 0.5f) {
                speedTargetY = 0.5f;
            }
            else if (speedTargetY < -0.5f) {
                speedTargetY = -0.5f;
            }
        }

        input = new Vector2(speedTargetX, speedTargetY);
        if (input.sqrMagnitude > 1.0f) {
            input.Normalize();
        }
    }

    void FixedUpdate() {
        MovementChange moveChange = currentMoveState.CalculateAcceleration(input, localVelocity);

        //localVelocity = new Vector3(
        //    localVelocity.x + lateralAcceleration * Time.fixedDeltaTime,
        //    localVelocity.y,
        //    localVelocity.z + forwardAcceleration * Time.fixedDeltaTime
        //);

        if (localVelocity.Equals(moveChange.localVelocityOverride)) {
            localVelocity += moveChange.localAcceleration * Time.fixedDeltaTime;
        }
        else {
            if (localVelocity.x == moveChange.localVelocityOverride.x) {
                localVelocity.x += moveChange.localAcceleration.x * Time.fixedDeltaTime;
            }
            else {
                localVelocity.x = moveChange.localVelocityOverride.x;
            }
            if (localVelocity.y == moveChange.localVelocityOverride.y) {
                localVelocity.y += moveChange.localAcceleration.y * Time.fixedDeltaTime;
            }
            else {
                localVelocity.y = moveChange.localVelocityOverride.y;
            }
            if (localVelocity.z == moveChange.localVelocityOverride.z) {
                localVelocity.z += moveChange.localAcceleration.z * Time.fixedDeltaTime;
            }
            else {
                localVelocity.z = moveChange.localVelocityOverride.z;
            }
        }

        rigidbody.MoveRotation(Quaternion.AngleAxis(screenMouseRatio * mouseSensitivity * mouseX * Time.fixedDeltaTime, Vector3.up) * rigidbody.rotation);
        rigidbody.MovePosition(rigidbody.position + (rigidbody.rotation * localVelocity) * Time.fixedDeltaTime);
    }
}
