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

    [System.Serializable]
    public struct CharacterMovementCharacteristics {
        public MovementCharacteristics forward;
        public MovementCharacteristics reverse;
        public MovementCharacteristics lateral;
    }

    public const float EPSILON = 1e-6f;

    public float mouseSensitivity = 100.0f;
    public float screenMouseRatio = 1.777f;

    public Vector2 deadzone = new Vector2(0.01f, 0.01f);

    public IMovementState runningState;

    public WeaponController weaponController;

    public float groundCheckDistance = 0.1f;
    public float maxTurnSpeed = 1.0f;

    private Rigidbody rigidbody;
    private Animator animator;

    private IMovementState currentMoveState;
    private bool jump = false;
    private bool climbing = false;
    private bool climbingLowerTrigger = false;

    private bool grounded = true;
    private Vector3 groundNormal;
    private Vector3 groundPoint;

    private bool screenControl = true;
    private float aimPitch = 0f;

    private float speedTargetX;
    private float speedTargetY;
    private float mouseX;
    private float mouseY;

    private Vector3 localVelocity = Vector3.zero;
    private Vector2 input = Vector2.zero;

    private LayerMask mask;

    private class VelocityBuffer {
        private Vector3[] buffer;
        private int index = 0;

        public VelocityBuffer(int size) {
            buffer = new Vector3[size];
            for (int n = 0; n < buffer.Length; ++n) {
                buffer[n] = Vector3.zero;
            }
            index = 0;
        }

        public void AddVelocity(Vector3 velocity) {
            buffer[index] = velocity;
            index = (index + 1) % buffer.Length;
        }

        public Vector3 Average() {
            Vector3 current = Vector3.zero;
            foreach (Vector3 vec in buffer) {
                current += vec;
            }
            current /= buffer.Length;
            return current;
        }

        public Vector3 WeightedAverage() {
            Vector3 current = Vector3.zero;
            int totalWeight = 0;
            for (int n = 0, index = this.index; n < buffer.Length; ++n) {
                current += (buffer.Length - n) * buffer[(index + n) % buffer.Length];
                totalWeight += buffer.Length - n;
            }
            current /= totalWeight;
            return current;
        }
    }

    private VelocityBuffer velBuffer;

    public Quaternion AimDirection() {
        return Quaternion.Euler(-aimPitch, transform.eulerAngles.y, 0f);
    }

    public float AimPitch() {
        return aimPitch;
    }

    private void Start() {
        rigidbody = gameObject.GetComponent<Rigidbody>();
        animator = gameObject.GetComponent<Animator>();

        runningState = gameObject.GetComponent<Running>() as IMovementState;

        currentMoveState = runningState;

        EventManager.StartListening<WeaponFirePrimary>(
            new UnityEngine.Events.UnityAction(WeaponFirePrimaryCallbackTest));
        EventManager.StartListening<WeaponFireSecondary, float>(
            new UnityEngine.Events.UnityAction<float>(WeaponFirePrimaryCallbackTest));

        Cursor.lockState = CursorLockMode.Locked;
        screenControl = true;

        mask = LayerMask.GetMask("Static Level Geometry", "Moving Level Geometry");
        velBuffer = new VelocityBuffer(16);
    }

    private void WeaponFirePrimaryCallbackTest() {
        Debug.Log("Fire Weapon Primary");
    }

    private void WeaponFirePrimaryCallbackTest(float holdTime) {
        Debug.Log("Fire Weapon Secondary: " + holdTime);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            if (!jump) {
                jump = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            transform.position = transform.position + new Vector3(0f, 20f, 0f);
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

        if (screenControl) {
            mouseX = Input.GetAxis("Mouse X");
            mouseY = Input.GetAxis("Mouse Y");

            aimPitch += mouseSensitivity * mouseY * Time.deltaTime;
            if (aimPitch > 80f) {
                aimPitch = 80f;
            }
            else if (aimPitch < -80f) {
                aimPitch = -80f;
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape)) {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            screenControl = false;
            mouseX = mouseY = 0f;
        }

        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            Cursor.lockState = CursorLockMode.Locked;
            screenControl = true;
        }

        transform.Rotate(Vector3.up, screenMouseRatio * mouseSensitivity * mouseX * Time.deltaTime);
        float extraRotation = Mathf.Clamp(mouseX, -maxTurnSpeed, maxTurnSpeed);

        rigidbody.velocity = Quaternion.AngleAxis(screenMouseRatio * mouseSensitivity * extraRotation * Time.deltaTime, Vector3.up) * rigidbody.velocity;
    }

    private void FixedUpdate() {

        Vector3 localRigidbodyVelocity = Quaternion.Inverse(rigidbody.rotation) * Vector3.ProjectOnPlane(rigidbody.velocity, Vector3.up);
        Debug.DrawRay(rigidbody.position + 0.5f * Vector3.up, Quaternion.Inverse(rigidbody.rotation) * rigidbody.velocity, Color.green);

        CheckGrounded();

            // this is wrong.
        //input = Vector3.ProjectOnPlane(input, groundNormal);
        //Vector3 desiredMove = Vector3.ProjectOnPlane(input, groundNormal).normalized;

        //if ((Quaternion.Inverse(rigidbody.rotation) * rigidbody.velocity).sqrMagnitude > 1f) {
        //    rigidbody.AddForce(desiredMove, ForceMode.Acceleration);
        //}

        float rotationAmount = Mathf.Atan2(input.x, input.y);
        float forwardAmount = input.y;

        //float turnSpeed = Mathf.Lerp(m_StationaryTurnSpeed, m_MovingTurnSpeed, m_ForwardAmount);

        //Debug.Log(Quaternion.Inverse(rigidbody.rotation) * rigidbody.velocity == transform.InverseTransformDirection(rigidbody.velocity));
        Debug.Log(localRigidbodyVelocity);

        MovementChange moveChange;
        if (grounded) {
            moveChange = currentMoveState.CalculateAcceleration(input, localRigidbodyVelocity);

            if (jump) {
                moveChange.localVelocityOverride.y = 4.0f;
                grounded = false;
                jump = false;
            }
        }
        else {
            moveChange = new MovementChange(-0.7f * Vector3.up, localRigidbodyVelocity);
        }

        if (moveChange.localVelocityOverride == localRigidbodyVelocity) {
            Debug.Log(rigidbody.velocity.x + ", " + rigidbody.velocity.y + ", " + rigidbody.velocity.z);
            rigidbody.AddRelativeForce(moveChange.localAcceleration, ForceMode.Acceleration);
                // or
            //rigidbody.AddForce(rigidbody.rotation * moveChange.localAcceleration, ForceMode.Acceleration);

            if (Input.GetKey(KeyCode.Alpha7)) Debug.Break();
            //rigidbody.velocity = Quaternion.AngleAxis(screenMouseRatio * mouseSensitivity * extraRotation, Vector3.up) * rigidbody.velocity;

            //rigidbody.MoveRotation(Quaternion.AngleAxis(screenMouseRatio * mouseSensitivity * mouseX * Time.fixedDeltaTime, Vector3.up) * rigidbody.rotation);
        }
        else {
            Vector3 localVelocityOverride = new Vector3(localRigidbodyVelocity.x, localRigidbodyVelocity.y, localRigidbodyVelocity.z);

            rigidbody.AddRelativeForce(moveChange.localAcceleration, ForceMode.Acceleration);
            rigidbody.MoveRotation(Quaternion.AngleAxis((screenMouseRatio * mouseSensitivity * mouseX) * Time.fixedDeltaTime, Vector3.up) * rigidbody.rotation);

            if (moveChange.localVelocityOverride.x != localRigidbodyVelocity.x) {
                localVelocityOverride.x = moveChange.localVelocityOverride.x;
            }
            if (moveChange.localVelocityOverride.y != localRigidbodyVelocity.y) {
                localVelocityOverride.y = moveChange.localVelocityOverride.y;
            }
            if (moveChange.localVelocityOverride.z != localRigidbodyVelocity.z) {
                localVelocityOverride.z = moveChange.localVelocityOverride.z;
            }

            rigidbody.velocity = rigidbody.rotation * localVelocityOverride;
        }

        velBuffer.AddVelocity(rigidbody.velocity);
    }

    private void CheckGrounded() {
        RaycastHit hitInfo;

        Debug.DrawLine(transform.position + (Vector3.up * 0.1f), transform.position + (Vector3.up * 0.1f) + (Vector3.down * groundCheckDistance), Color.yellow);

        if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, groundCheckDistance, mask)) {
            groundNormal = hitInfo.normal;
            groundPoint = hitInfo.point;
            grounded = true;
            //animator.applyRootMotion = true;
        }
        else {
            groundNormal = Vector3.zero;
            grounded = false;
            //animator.applyRootMotion = false;
        }
    }

    //private void OnAnimatorMove() {
    //    if (grounded) {
    //        Vector3 newVelocity = animator.deltaPosition / Time.deltaTime;
    //        newVelocity.y = rigidbody.velocity.y;
    //        //rigidbody.velocity = newVelocity;
    //    }
    //}
}
