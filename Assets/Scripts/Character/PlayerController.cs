using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IPlayerAim {

    private Rigidbody rigidbody;
    private Animator animator;

    public float mouseSensitivity = 1.0f;
    public float screenMouseRatio = 1.777f;

    public Vector3 charVelocityRun;

    private float aimPitch = 0f;

    private float moveZ;
    private float moveX;
    private float mouseX;
    private float mouseY;

    private int moveMode = 0;
    private const int MOVE_MODE_RUN = 0;
    private const int MOVE_MODE_CLIMB = 2;
    private bool jump = false;
    private bool climbing = false;
    private bool climbingLowerTrigger = false;

    void Start() {
        rigidbody = gameObject.GetComponent<Rigidbody>();
        animator = gameObject.GetComponent<Animator>();
    }

    public Quaternion AimDirection() {
        return Quaternion.Euler(-aimPitch, transform.eulerAngles.y, 0f);
    }

    private bool ctrlRbJump = false;

    public Vector3 test_rigidbody_velocity;

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
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            transform.position = transform.position + new Vector3(0f, 20f, 0f);
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

        //transform.rotation = Quaternion.AngleAxis(screenMouseRatio * mouseSensitivity * mouseX * Time.deltaTime, Vector3.up) * transform.rotation;
        //transform.position = transform.position + (transform.forward * moveZ * charVelocityRun.z + transform.right * moveX * charVelocityRun.x) * Time.deltaTime;
    }

    private Vector3 velocity;

    void FixedUpdate() {
        //rigidbody.transform.position += transform.rotation * (0.5f * Vector3.forward * Time.deltaTime);

        //rigidbody.MoveRotation(Quaternion.AngleAxis(screenMouseRatio * mouseSensitivity * mouseX * Time.fixedDeltaTime, Vector3.up) * rigidbody.rotation);
        //rigidbody.MovePosition(transform.position + (transform.forward * moveZ * charVelocityRun.z + transform.right * moveX * charVelocityRun.x) * Time.fixedDeltaTime);
        rigidbody.MoveRotation(Quaternion.AngleAxis(screenMouseRatio * mouseSensitivity * mouseX * Time.fixedDeltaTime, Vector3.up) * rigidbody.rotation);
        rigidbody.MovePosition(rigidbody.position + (transform.forward * moveZ * charVelocityRun.z + transform.right * moveX * charVelocityRun.x) * Time.fixedDeltaTime);

        //rigidbody.velocity = rigidbody.rotation * (new Vector3(moveX * charVelocityRun.x, rigidbody.velocity.y, moveZ * charVelocityRun.z));
        //rigidbody.position = rigidbody.position + rigidbody.rotation * (new Vector3(moveX * charVelocityRun.x, rigidbody.velocity.y, moveZ * charVelocityRun.z) * Time.fixedDeltaTime);

        if (ctrlRbJump) {
            rigidbody.velocity = new Vector3(rigidbody.velocity.x, 4.0f, rigidbody.velocity.z);
            ctrlRbJump = false;
            jump = false;
        }

        test_rigidbody_velocity = rigidbody.velocity;
    }
}
