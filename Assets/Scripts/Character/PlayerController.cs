using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IPlayerAim {

    public enum MoveMode : int {
        Running = 0,
        Falling,
        Climbing,
        Landing
    };

    private Rigidbody rigidbody;
    private Animator animator;

    public float mouseSensitivity = 1.0f;
    public float screenMouseRatio = 1.777f;

    [System.Serializable]
    public struct MovementCharacteristics {
        public AnimationCurve accelerationCurve;
        public AnimationCurve decelerationCurve;
        public float maxSpeed;
        public float accelerationScalar;

        public float Acceleration(float velocity) {
            return accelerationScalar * accelerationCurve.Evaluate(velocity / maxSpeed);
        }

        public float Deceleration(float velocity) {
            return accelerationScalar * decelerationCurve.Evaluate(1.0f - velocity / maxSpeed);
        }
    }

    [System.Serializable]
    public struct CharacterMovementCharacteristics {
        public MovementCharacteristics forward;
        public MovementCharacteristics reverse;
        public MovementCharacteristics lateral;
    }

    public float velocityCurveTraverseSpeed = 1f;

    public CharacterMovementCharacteristics runningCharacteristics;

    private float aimPitch = 0f;

    private float speedTargetX;
    private float speedTargetY;
    private float mouseX;
    private float mouseY;

    private MoveMode moveMode = MoveMode.Running;
    private bool jump = false;
    private bool climbing = false;
    private bool climbingLowerTrigger = false;

    private Vector3 playerVelocityLocal = Vector3.zero;

    void Start() {
        rigidbody = gameObject.GetComponent<Rigidbody>();
        animator = gameObject.GetComponent<Animator>();
    }

    public Quaternion AimDirection() {
        return Quaternion.Euler(-aimPitch, transform.eulerAngles.y, 0f);
    }

    private bool ctrlRbJump = false;

    void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            if (!jump) {
                ctrlRbJump = jump = true;
                if (moveMode == MoveMode.Running) {
                    moveMode = MoveMode.Climbing;
                }
                else if (moveMode == MoveMode.Climbing) {
                    moveMode = MoveMode.Running;
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

        //transform.rotation = Quaternion.AngleAxis(screenMouseRatio * mouseSensitivity * mouseX * Time.deltaTime, Vector3.up) * transform.rotation;
        //transform.position = transform.position + (transform.forward * moveZ * charVelocityRun.z + transform.right * moveX * charVelocityRun.x) * Time.deltaTime;
    }

    private Vector3 velocity = Vector3.zero;
    private Vector2 input = Vector2.zero;

    private float forwardAcceleration = 0f;
    private float lateralAcceleration = 0f;

    private const float EPSILON = 1e-6f;

    void FixedUpdate() {

        /*float*/
        forwardAcceleration = 0f;
        /*float*/
        lateralAcceleration = 0f;

        if (velocity.x > EPSILON) {
            if (input.x * runningCharacteristics.lateral.maxSpeed >= velocity.x) {
                Debug.Log("1");
                lateralAcceleration = runningCharacteristics.lateral.Acceleration(velocity.x);
            }
            else if (input.x * runningCharacteristics.lateral.maxSpeed < velocity.x) {
                Debug.Log("2");
                lateralAcceleration = -runningCharacteristics.lateral.Deceleration(velocity.x);
            }
        }
        else if (velocity.x < -EPSILON) {
            if (input.x * runningCharacteristics.lateral.maxSpeed <= velocity.x) {
                Debug.Log("3");
                lateralAcceleration = -runningCharacteristics.lateral.Acceleration(-velocity.x);
            }
            else if (input.x * runningCharacteristics.lateral.maxSpeed > velocity.x) {
                Debug.Log("4");
                lateralAcceleration = runningCharacteristics.lateral.Deceleration(-velocity.x);
            }
        }
        else {
            if (Mathf.Abs(input.x * runningCharacteristics.lateral.maxSpeed - velocity.x) > EPSILON) {
                Debug.Log("5");
                lateralAcceleration = Mathf.Sign(input.x) * runningCharacteristics.lateral.Acceleration(Mathf.Abs(velocity.x));
            }
            else {
                lateralAcceleration = 0f;
                    // TODO: THIS IS A MISTAKE, don't do this, replace in future
                velocity.x = 0f;
            }
        }

        if (velocity.z > EPSILON) {
            if (input.y * runningCharacteristics.forward.maxSpeed >= velocity.z) {
                forwardAcceleration = runningCharacteristics.forward.Acceleration(velocity.z);
            }
            else if (input.y * runningCharacteristics.forward.maxSpeed < velocity.z) {
                forwardAcceleration = -runningCharacteristics.forward.Deceleration(velocity.z);
            }
        }
        else if (velocity.z < -EPSILON) {
            if (input.y * runningCharacteristics.reverse.maxSpeed <= velocity.z) {
                forwardAcceleration = -runningCharacteristics.reverse.Acceleration(-velocity.z);
            }
            else if (input.y * runningCharacteristics.reverse.maxSpeed > velocity.z) {
                forwardAcceleration = runningCharacteristics.reverse.Deceleration(-velocity.z);
            }
        }
        else {
            if (Mathf.Abs(input.y * runningCharacteristics.forward.maxSpeed - velocity.z) > EPSILON) {
                forwardAcceleration = Mathf.Sign(input.y) * runningCharacteristics.forward.Acceleration(Mathf.Abs(velocity.z));
            }
            else {
                forwardAcceleration = 0f;
            }
        }

        velocity = new Vector3(
            velocity.x + lateralAcceleration * Time.fixedDeltaTime,
            velocity.y,
            velocity.z + forwardAcceleration * Time.fixedDeltaTime
        );

        //if (velocity.x > maxCharVelocityRun.x) velocity.x = maxCharVelocityRun.x;
        //if (velocity.z > maxCharVelocityRun.z) velocity.z = maxCharVelocityRun.z;

        rigidbody.MoveRotation(Quaternion.AngleAxis(screenMouseRatio * mouseSensitivity * mouseX * Time.fixedDeltaTime, Vector3.up) * rigidbody.rotation);
        //rigidbody.MovePosition(rigidbody.position + (transform.forward * input.y * maxCharVelocityRun.z + transform.right * input.x * maxCharVelocityRun.x) * Time.fixedDeltaTime);
        rigidbody.MovePosition(rigidbody.position + (rigidbody.rotation * velocity) * Time.fixedDeltaTime);

        //rigidbody.velocity = rigidbody.rotation * (new Vector3(moveX * charVelocityRun.x, rigidbody.velocity.y, moveZ * charVelocityRun.z));
        //rigidbody.position = rigidbody.position + rigidbody.rotation * (new Vector3(moveX * charVelocityRun.x, rigidbody.velocity.y, moveZ * charVelocityRun.z) * Time.fixedDeltaTime);

        if (ctrlRbJump) {
            rigidbody.velocity = new Vector3(rigidbody.velocity.x, 4.0f, rigidbody.velocity.z);
            ctrlRbJump = false;
            jump = false;
        }
    }
}
