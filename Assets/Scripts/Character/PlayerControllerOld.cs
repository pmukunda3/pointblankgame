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

public class PlayerControllerOld : MonoBehaviour, IPlayerAim {

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

    public float mouseSensitivity = 100.0f;
    public float screenMouseRatio = 1.777f;

    public Vector2 deadzone = new Vector2(0.01f, 0.01f);

    public IMovementState runningState;

    public WeaponController weaponController;

    public float groundCheckDistance = 0.1f;

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

        Cursor.lockState = CursorLockMode.Locked;
        screenControl = true;

        mask = LayerMask.GetMask("Static Level Geometry", "Moving Level Geometry");
        mask = LayerMask.GetMask("Player Character");
    }

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

    public Quaternion AimDirection() {
        return Quaternion.Euler(-aimPitch, transform.eulerAngles.y, 0f);
    }

    public float AimPitch() {
        return aimPitch;
    }

    private LayerMask mask;

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
    }

    private void FixedUpdate() {
        CheckGrounded();

        MovementChange moveChange = currentMoveState.CalculateAcceleration(input, Quaternion.Inverse(rigidbody.rotation) * rigidbody.velocity);

        localVelocity += moveChange.localAcceleration * Time.fixedDeltaTime;

        if (jump) {
            jump = false;
            localVelocity.y += 4.0f;
        }

        rigidbody.MoveRotation(Quaternion.AngleAxis(screenMouseRatio * mouseSensitivity * mouseX * Time.fixedDeltaTime, Vector3.up) * rigidbody.rotation);

        if (grounded) {
            //rigidbody.MovePosition(Vector3.ProjectOnPlane(rigidbody.position - groundPoint, groundNormal) + rigidbody.position);
            rigidbody.MovePosition(groundPoint);
            localVelocity.y = 0f;
            rigidbody.MovePosition(rigidbody.position + (rigidbody.rotation * localVelocity) * Time.fixedDeltaTime);
        }
        else {
            localVelocity.y -= 9.81f * Time.fixedDeltaTime;
        }

        //localVelocity -= Vector3.Project(localVelocity, Quaternion.Inverse(rigidbody.rotation) * rbCollisionContact);
        localVelocity -= Quaternion.Inverse(rigidbody.rotation) * rbCollisionContact.normalized * localVelocity.magnitude;

        Debug.DrawRay(rigidbody.position + 0.1f * Vector3.up, rigidbody.rotation * localVelocity, Color.green);
        Debug.DrawRay(rigidbody.position, rbCollisionContact.normalized * localVelocity.magnitude, Color.blue);
        rbCollisionContact = Vector3.zero;
    }

    private Vector3 rbCollisionContact = Vector3.zero;
    private List<Collider> contactColliders = new List<Collider>(64);

    private void OnCollisionStay(Collision collision) {
        ContactPoint[] contacts = new ContactPoint[32];
        collision.GetContacts(contacts);

        int n = 0;
        for (n = 0; n < contacts.Length && contacts[n].otherCollider != null; ++n) {
            ContactPoint contact = contacts[n];
            Debug.DrawRay(contact.point, -10f * contact.separation * contact.normal, Color.red);
            rbCollisionContact += contact.separation * contact.normal;
        }

        //if (n > 1) Debug.Break();
    }

    private void OnCollisionEnter(Collision collision) {
        contactColliders.Add(collision.collider);
        //foreach (ContactPoint contact in collision.contacts) {
            
        //}
    }

    private void OnCollisionExit(Collision collision) {
        contactColliders.Remove(collision.collider);
    }

    private void CheckGrounded() {
        RaycastHit hitInfo;

        Debug.DrawLine(transform.position + (Vector3.up * 0.1f), transform.position + (Vector3.up * 0.1f) + (Vector3.down * groundCheckDistance), Color.yellow);

        if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, mask)) {
            groundNormal = hitInfo.normal;
            groundPoint = hitInfo.point;
            grounded = true;
            jump = false;
            //animator.applyRootMotion = true;
        }
        else {
            groundNormal = Vector3.zero;
            grounded = false;
            //animator.applyRootMotion = false;
        }
    }

    private void OnAnimatorMove() {
        if (!grounded) {
            rigidbody.velocity *= 0.5f;
        }
    }
}
