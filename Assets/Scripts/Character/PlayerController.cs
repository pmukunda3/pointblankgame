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

    public Vector2 deadzone = new Vector2(0.01f, 0.01f);

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
    }

    private Vector3 localVelocity = Vector3.zero;
    private Vector2 input = Vector2.zero;

    private float forwardAcceleration = 0f;
    private float lateralAcceleration = 0f;

    private const float EPSILON = 1e-6f;

    void FixedUpdate() {

        /*float*/ forwardAcceleration = 0f;
        /*float*/ lateralAcceleration = 0f;

        if (localVelocity.x > EPSILON) {
            if (input.x * runningCharacteristics.lateral.maxSpeed >= localVelocity.x) {
                Debug.Log("1");
                lateralAcceleration = runningCharacteristics.lateral.Acceleration(localVelocity.x);
                if (localVelocity.x + lateralAcceleration * Time.fixedDeltaTime > input.x * runningCharacteristics.lateral.maxSpeed + EPSILON) {
                    lateralAcceleration = (input.x * runningCharacteristics.lateral.maxSpeed - localVelocity.x) / Time.fixedDeltaTime;
                }
            }
            else if (input.x * runningCharacteristics.lateral.maxSpeed < localVelocity.x) {
                Debug.Log("2");
                lateralAcceleration = -runningCharacteristics.lateral.Deceleration(localVelocity.x);
                if (localVelocity.x + lateralAcceleration * Time.fixedDeltaTime < input.x * runningCharacteristics.lateral.maxSpeed + EPSILON) {
                    lateralAcceleration = (input.x * runningCharacteristics.lateral.maxSpeed - localVelocity.x) / Time.fixedDeltaTime;
                }
            }
        }
        else if (localVelocity.x < -EPSILON) {
            if (input.x * runningCharacteristics.lateral.maxSpeed <= localVelocity.x) {
                Debug.Log("3");
                lateralAcceleration = -runningCharacteristics.lateral.Acceleration(-localVelocity.x);
                if (localVelocity.x + lateralAcceleration * Time.fixedDeltaTime < input.x * runningCharacteristics.lateral.maxSpeed - EPSILON) {
                    lateralAcceleration = (input.x * runningCharacteristics.lateral.maxSpeed - localVelocity.x) / Time.fixedDeltaTime;
                }
            }
            else if (input.x * runningCharacteristics.lateral.maxSpeed > localVelocity.x) {
                Debug.Log("4");
                lateralAcceleration = runningCharacteristics.lateral.Deceleration(-localVelocity.x);
                if (localVelocity.x + lateralAcceleration * Time.fixedDeltaTime > input.x * runningCharacteristics.lateral.maxSpeed - EPSILON) {
                    lateralAcceleration = (input.x * runningCharacteristics.lateral.maxSpeed - localVelocity.x) / Time.fixedDeltaTime;
                }
            }
        }
        else {
            if (Mathf.Abs(input.x * runningCharacteristics.lateral.maxSpeed - localVelocity.x) > EPSILON) {
                Debug.Log("5");
                lateralAcceleration = Mathf.Sign(input.x) * runningCharacteristics.lateral.Acceleration(Mathf.Abs(localVelocity.x));
            }
            else {
                Debug.Log("6");
                lateralAcceleration = 0f;
                    // TODO: THIS IS A MISTAKE, don't do this, replace in future
                localVelocity.x = 0f;
            }
        }

        if (localVelocity.z > EPSILON) {
            if (input.y * runningCharacteristics.forward.maxSpeed >= localVelocity.z) {
                forwardAcceleration = runningCharacteristics.forward.Acceleration(localVelocity.z);
                if (localVelocity.z + forwardAcceleration * Time.fixedDeltaTime > input.y * runningCharacteristics.forward.maxSpeed + EPSILON) {
                    forwardAcceleration = (input.y * runningCharacteristics.forward.maxSpeed - localVelocity.z) / Time.fixedDeltaTime;
                }
            }
            else if (input.y * runningCharacteristics.forward.maxSpeed < localVelocity.z) {
                forwardAcceleration = -runningCharacteristics.forward.Deceleration(localVelocity.z);
                if (localVelocity.z + forwardAcceleration * Time.fixedDeltaTime < input.y * runningCharacteristics.forward.maxSpeed + EPSILON) {
                    forwardAcceleration = (input.y * runningCharacteristics.forward.maxSpeed - localVelocity.z) / Time.fixedDeltaTime;
                }
            }
        }
        else if (localVelocity.z < -EPSILON) {
            if (input.y * runningCharacteristics.reverse.maxSpeed <= localVelocity.z) {
                forwardAcceleration = -runningCharacteristics.reverse.Acceleration(-localVelocity.z);
                if (localVelocity.z + forwardAcceleration * Time.fixedDeltaTime < input.y * runningCharacteristics.reverse.maxSpeed - EPSILON) {
                    forwardAcceleration = (input.y * runningCharacteristics.reverse.maxSpeed - localVelocity.z) / Time.fixedDeltaTime;
                }
            }
            else if (input.y * runningCharacteristics.reverse.maxSpeed > localVelocity.z) {
                forwardAcceleration = runningCharacteristics.reverse.Deceleration(-localVelocity.z);
                if (localVelocity.z + forwardAcceleration * Time.fixedDeltaTime > input.y * runningCharacteristics.reverse.maxSpeed - EPSILON) {
                    forwardAcceleration = (input.y * runningCharacteristics.reverse.maxSpeed - localVelocity.z) / Time.fixedDeltaTime;
                }
            }
        }
        else {
            if (Mathf.Abs(input.y * runningCharacteristics.forward.maxSpeed - localVelocity.z) > EPSILON) {
                forwardAcceleration = Mathf.Sign(input.y) * runningCharacteristics.forward.Acceleration(Mathf.Abs(localVelocity.z));
            }
            else {
                forwardAcceleration = 0f;
                localVelocity.z = 0f;
            }
        }

        localVelocity = new Vector3(
            localVelocity.x + lateralAcceleration * Time.fixedDeltaTime,
            localVelocity.y,
            localVelocity.z + forwardAcceleration * Time.fixedDeltaTime
        );

        if (localVelocity.x > runningCharacteristics.lateral.maxSpeed) localVelocity.x = runningCharacteristics.lateral.maxSpeed;
        if (localVelocity.z > runningCharacteristics.forward.maxSpeed) localVelocity.z = runningCharacteristics.forward.maxSpeed;

        rigidbody.MoveRotation(Quaternion.AngleAxis(screenMouseRatio * mouseSensitivity * mouseX * Time.fixedDeltaTime, Vector3.up) * rigidbody.rotation);
        rigidbody.MovePosition(rigidbody.position + (rigidbody.rotation * localVelocity) * Time.fixedDeltaTime);

        if (ctrlRbJump) {
            rigidbody.velocity = new Vector3(rigidbody.velocity.x, 4.0f, rigidbody.velocity.z);
            ctrlRbJump = false;
            jump = false;
        }
    }
}
