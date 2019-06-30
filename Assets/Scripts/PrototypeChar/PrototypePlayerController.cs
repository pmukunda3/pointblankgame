using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Animator))]
public class PrototypePlayerController : MonoBehaviour, IPlayerAim {

    private Rigidbody rigidbody;
    private Animator animator;

    public TriggerCallback climbTrigger;
    public TriggerCallback climbCrestTrigger;
    public TriggerCallback landTrigger;

    public float mouseSensitivity = 1.0f;
    public float screenMouseRatio = 1.777f;

    public Vector3 charVelocityRun;
    public Vector3 charVelocityClimb;

    private float aimPitch = 0f;

    private float moveZ;
    private float moveX;

    private int moveMode = 0;
    private const int MOVE_MODE_RUN = 0;
    private const int MOVE_MODE_CLIMB = 2;
    private bool jump = false;
    private bool climbing = false;
    private bool climbingLowerTrigger = false;

    void Start() {
        rigidbody = gameObject.GetComponent<Rigidbody>();
        animator = gameObject.GetComponent<Animator>();

        climbTrigger.PassCallback(OnTriggerEnterClimb, OnTriggerExitClimb);
        climbCrestTrigger.PassCallback(OnTriggerEnterClimbCrest, OnTriggerExitClimbCrest);
        landTrigger.PassCallback(OnTriggerEnterLand);

        //animator.SetFloat("velLocalZ", 0.2f);
        animator.SetBool("idle", true);
        animator.SetInteger("moveMode", moveMode);
    }

    public Quaternion AimDirection() {
        return Quaternion.Euler(-aimPitch, transform.eulerAngles.y, 0f);
    }

    public float AimPitch() {
        return aimPitch;
    }

    public float AimYaw() {
        return transform.eulerAngles.y;
    }

    public Quaternion AimYawQuaternion() {
        return Quaternion.Euler(0f, transform.eulerAngles.y, 0f);
    }

    private bool ctrlRbJump = false;

    void Update() {
        if (Input.GetKey(KeyCode.W)) {
            // TODO: Fix these
        }

        if (Input.GetKey(KeyCode.A)) {
            // TODO: Fix these
        }

        if (Input.GetKey(KeyCode.S)) {
            // TODO: Fix these
        }

        if (Input.GetKey(KeyCode.D)) {
            // TODO: Fix these
        }

        if (Input.GetKeyDown(KeyCode.Space)) {
            if (!jump) {
                ctrlRbJump = jump = true;
                if (moveMode == MOVE_MODE_RUN) {
                    moveMode = MOVE_MODE_CLIMB;
                }
                else if (moveMode == MOVE_MODE_CLIMB) {
                    moveMode = MOVE_MODE_RUN;
                }
                animator.SetInteger("moveMode", moveMode);
            }
        }

        if (Input.GetKeyDown(KeyCode.Keypad1)) {
            moveMode = 0;
            animator.SetInteger("moveMode", moveMode);
        }

        if (Input.GetKeyDown(KeyCode.Keypad2)) {
            moveMode = 2;
            animator.SetInteger("moveMode", moveMode);
        }

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        transform.rotation = Quaternion.AngleAxis(screenMouseRatio * mouseSensitivity * mouseX * Time.deltaTime, Vector3.up) * transform.rotation;
        aimPitch += mouseSensitivity * mouseY * Time.deltaTime;
        if (aimPitch > 90f) {
            aimPitch = 90f;
        }
        else if (aimPitch < -90f) {
            aimPitch = -90f;
        }

        moveZ = Input.GetAxis("Vertical");
        moveX = Input.GetAxis("Horizontal");

        if (rigidbody.velocity.sqrMagnitude < 0.04f) {
            animator.SetBool("idle", true);
        }
        else {
            animator.SetBool("idle", false);
        }
    }

    private Vector3 velocity;

    void FixedUpdate() {
        //rigidbody.transform.position += transform.rotation * (0.5f * Vector3.forward * Time.deltaTime);

        if (!climbing && !climbingLowerTrigger) {
            rigidbody.velocity = transform.rotation * (new Vector3(moveX * charVelocityRun.x, rigidbody.velocity.y, moveZ * charVelocityRun.z));
        }
        else if (!climbing && climbingLowerTrigger) {
            rigidbody.AddForce(9.81f * Vector3.up);
            rigidbody.velocity = transform.rotation * (new Vector3(0f, 1.5f, 0.6f));
        }
        else if (climbing) {
            rigidbody.AddForce(9.81f * Vector3.up);
            rigidbody.velocity = transform.rotation * (new Vector3(moveX * charVelocityClimb.x, moveZ * charVelocityClimb.y, 0.002f));
        }

        if (ctrlRbJump) {
            rigidbody.velocity = new Vector3(rigidbody.velocity.x, 4.0f, rigidbody.velocity.z);
            ctrlRbJump = false;
        }
    }

    void OnTriggerEnterClimb(GameObject triggerRef, Collider other) {
        if (triggerRef.CompareTag("climbTrigger")) {
            Debug.Log("CLIMB");
            climbing = true;
        }
    }

    void OnTriggerExitClimb(GameObject triggerRef, Collider other) {
        climbing = false;
    }

    void OnTriggerEnterClimbCrest(GameObject triggerRef, Collider other) {
        climbingLowerTrigger = true;
    }

    void OnTriggerExitClimbCrest(GameObject triggerRef, Collider other) {
        climbingLowerTrigger = false;
    }

    void OnTriggerEnterLand(GameObject triggerRef, Collider other) {
        if (triggerRef.CompareTag("landTrigger")) {
            if (!climbing) {
                Debug.Log("LAND");
                jump = false;
                moveMode = MOVE_MODE_RUN;
                animator.SetInteger("moveMode", moveMode);
            }
        }
    }
}
