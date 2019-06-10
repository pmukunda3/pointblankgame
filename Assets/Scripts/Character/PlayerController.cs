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

    public Vector3 maxCharVelocityRun;

    [System.Serializable]
    public struct MovementCharacteristics {
        public AnimationCurve forwardAccelVelocityCurve;
        public AnimationCurve forwardDecelVelocityCurve;
        public AnimationCurve reverseAccelVelocityCurve;
        public AnimationCurve reverseDecelVelocityCurve;
        public AnimationCurve lateralAccelVelocityCurve;
        public AnimationCurve lateralDecelVelocityCurve;
    }

    public float velocityCurveTraverseSpeed = 1f;

    public MovementCharacteristics runningCharacteristics;

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

    private float velCurveTargetX = 0f;
    private float velCurveCurrentX = 0f;
    private float velCurveX = 0f;

    private float forwardAcceleration = 0f;
    private float lateralAcceleration = 0f;

    private const float EPSILON = 1e-6f;

    void FixedUpdate() {

        AnimationCurve forwardVelCurve = null;
        AnimationCurve lateralVelCurve = null;

        /*float*/ forwardAcceleration = 0f;
        /*float*/ lateralAcceleration = 0f;

        if (velocity.x > EPSILON) {
            if (input.x * maxCharVelocityRun.x >= velocity.x) {
                Debug.Log("1");
                Debug.Break();
                lateralVelCurve = runningCharacteristics.lateralAccelVelocityCurve;
                velCurveTargetX = lateralInverseVelCurve.Evaluate(input.x);

                //lateralAcceleration = Mathf.Sign(input.x - velocity.x / maxCharVelocityRun.x);
            }
            else if (input.x * maxCharVelocityRun.x < velocity.x) {
                Debug.Log("2");
                lateralVelCurve = runningCharacteristics.lateralDecelVelocityCurve;
                velCurveTargetX = lateralInverseVelCurve.Evaluate(input.x);
                //lateralAcceleration = Mathf.Sign(input.x - velocity.x / maxCharVelocityRun.x);
            }
        }
        else if (velocity.x < -EPSILON) {
            if (input.x * maxCharVelocityRun.x <= velocity.x) {
                Debug.Log("3");
                lateralInverseVelCurve = runningCharacteristics.lateralAccelVelocityCurve;
                lateralVelCurve = runningCharacteristics.lateralAccelVelocityCurve;
                lateralAcceleration = Mathf.Sign(input.x - velocity.x / maxCharVelocityRun.x);
            }
            else if (input.x * maxCharVelocityRun.x > velocity.x) {
                Debug.Log("4");
                lateralInverseVelCurve = runningCharacteristics.lateralDecelVelocityCurve;
                lateralVelCurve = runningCharacteristics.lateralDecelVelocityCurve;
                lateralAcceleration = Mathf.Sign(input.x - velocity.x / maxCharVelocityRun.x);
            }
        }
        else {
            lateralInverseVelCurve = runningCharacteristics.lateralAccelVelocityCurve;
            lateralVelCurve = runningCharacteristics.lateralAccelVelocityCurve;
            if (Mathf.Abs(input.x * maxCharVelocityRun.x - velocity.x) > EPSILON) {
                Debug.Log("5");
                Debug.Break();
                velCurveTargetX = lateralInverseVelCurve.Evaluate(input.x);
            }
        }

        if (velocity.z > EPSILON) {
            if (input.y * maxCharVelocityRun.z >= velocity.z) {
                forwardVelCurve = runningCharacteristics.forwardAccelVelocityCurve;
            }
            else if (input.y * maxCharVelocityRun.z < velocity.z) {
                forwardVelCurve = runningCharacteristics.forwardDecelVelocityCurve;
            }
        }
        else if (velocity.z < -EPSILON) {
            if (input.y * maxCharVelocityRun.z <= velocity.z) {
                forwardVelCurve = runningCharacteristics.forwardAccelVelocityCurve;
            }
            else if (input.y * maxCharVelocityRun.z > velocity.z) {
                forwardVelCurve = runningCharacteristics.forwardDecelVelocityCurve;
            }
        }
        else {
            forwardVelCurve = runningCharacteristics.forwardAccelVelocityCurve;
        }

        velCurveCurrentX = lateralInverseVelCurve.Evaluate(velocity.x / maxCharVelocityRun.x);

        if (velCurveTargetX > velCurveCurrentX + velocityCurveTraverseSpeed) {
            velCurveX = velCurveCurrentX + velocityCurveTraverseSpeed;
        }
        else {
            velCurveX = velCurveTargetX;
        }

        velocity = new Vector3(
            maxCharVelocityRun.x * lateralVelCurve.Evaluate(velCurveX),
            velocity.y,
            velocity.z + maxCharVelocityRun.z * (forwardAcceleration) * Time.fixedDeltaTime
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
