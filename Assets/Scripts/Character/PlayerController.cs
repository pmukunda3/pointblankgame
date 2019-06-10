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
        public AnimationCurve forwardAccel;
        public AnimationCurve forwardDecel;
        public AnimationCurve reverseAccel;
        public AnimationCurve reverseDecel;
        public AnimationCurve lateralAccel;
        public AnimationCurve lateralDecel;
    };

    public MovementCharacteristics runningCharacteristics;
    private MovementCharacteristics inverseRunningCharacteristics = new MovementCharacteristics();

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

        inverseRunningCharacteristics.lateralAccel = new AnimationCurve();
        inverseRunningCharacteristics.lateralDecel = new AnimationCurve();

        AnimationCurve[] curves = { runningCharacteristics.lateralAccel, runningCharacteristics.lateralDecel };
        AnimationCurve[] inverseCurves = { inverseRunningCharacteristics.lateralAccel, inverseRunningCharacteristics.lateralDecel };

        for (int curveIndex = 0; curveIndex < curves.Length; ++curveIndex) {
            AnimationCurve curve = curves[curveIndex];
            AnimationCurve inverseCurve = inverseCurves[curveIndex];
            for (int n = 0; n < curve.length; ++n) {
                Debug.Log("Key : " + curve.keys[n].time + ", Value : " + curve.keys[n].value);
                inverseCurve.AddKey(new Keyframe(curve.keys[n].value, curve.keys[n].time));
            }
        }
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

        input = new Vector2(speedTargetX, speedTargetY);
        if (input.sqrMagnitude > 1.0f) {
            input.Normalize();
        }

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

        //transform.rotation = Quaternion.AngleAxis(screenMouseRatio * mouseSensitivity * mouseX * Time.deltaTime, Vector3.up) * transform.rotation;
        //transform.position = transform.position + (transform.forward * moveZ * charVelocityRun.z + transform.right * moveX * charVelocityRun.x) * Time.deltaTime;
    }

    private Vector3 velocity = Vector3.zero;
    private Vector2 input = Vector2.zero;

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
                lateralVelCurve = inverseRunningCharacteristics.lateralAccel;
                lateralAcceleration = (lateralVelCurve.Evaluate(Mathf.Abs(velocity.x / maxCharVelocityRun.x) + EPSILON) - lateralVelCurve.Evaluate(Mathf.Abs(velocity.x / maxCharVelocityRun.x) - EPSILON))
                    / (2 * EPSILON) * maxCharVelocityRun.x;

                Debug.Log(lateralAcceleration);
            }
            else if (input.x * maxCharVelocityRun.x < velocity.x) {
                Debug.Log("2");
                lateralVelCurve = inverseRunningCharacteristics.lateralDecel;
                lateralAcceleration = -(lateralVelCurve.Evaluate(1.0f - Mathf.Abs(velocity.x / maxCharVelocityRun.x) + EPSILON) - lateralVelCurve.Evaluate(1.0f - Mathf.Abs(velocity.x / maxCharVelocityRun.x) - EPSILON))
                    / (2 * EPSILON) * maxCharVelocityRun.x;

                Debug.Log(lateralAcceleration);
            }
        }
        else if (velocity.x < -EPSILON) {
            if (input.x * maxCharVelocityRun.x <= velocity.x) {
                Debug.Log("3");
                lateralVelCurve = runningCharacteristics.lateralAccel;
                lateralAcceleration = -(lateralVelCurve.Evaluate(Mathf.Abs(velocity.x / maxCharVelocityRun.x) + EPSILON) - lateralVelCurve.Evaluate(Mathf.Abs(velocity.x / maxCharVelocityRun.x) - EPSILON))
                    / (2 * EPSILON) * maxCharVelocityRun.x;
            }
            else if (input.x * maxCharVelocityRun.x > velocity.x) {
                Debug.Log("4");
                lateralVelCurve = runningCharacteristics.lateralDecel;
                lateralAcceleration = (lateralVelCurve.Evaluate(Mathf.Abs(velocity.x / maxCharVelocityRun.x) + EPSILON) - lateralVelCurve.Evaluate(Mathf.Abs(velocity.x / maxCharVelocityRun.x) - EPSILON))
                    / (2 * EPSILON) * maxCharVelocityRun.x;
            }
        }
        else {
            lateralVelCurve = runningCharacteristics.lateralAccel;
            if (Mathf.Abs(input.x * maxCharVelocityRun.x - velocity.x) > EPSILON) {
                Debug.Log("5");
                lateralAcceleration = Mathf.Sign(input.x) * (lateralVelCurve.Evaluate(Mathf.Abs(velocity.x / maxCharVelocityRun.x) + EPSILON) - lateralVelCurve.Evaluate(Mathf.Abs(velocity.x / maxCharVelocityRun.x) - EPSILON))
                    / (2 * EPSILON) * maxCharVelocityRun.x;
            }
        }

        if (velocity.z > EPSILON) {
            if (input.y * maxCharVelocityRun.z >= velocity.z) {
                forwardVelCurve = runningCharacteristics.forwardAccel;
            }
            else if (input.y * maxCharVelocityRun.z < velocity.z) {
                forwardVelCurve = runningCharacteristics.forwardDecel;
            }
        }
        else if (velocity.z < -EPSILON) {
            if (input.y * maxCharVelocityRun.z <= velocity.z) {
                forwardVelCurve = runningCharacteristics.forwardAccel;
            }
            else if (input.y * maxCharVelocityRun.z > velocity.z) {
                forwardVelCurve = runningCharacteristics.forwardDecel;
            }
        }
        else {
            forwardVelCurve = runningCharacteristics.forwardAccel;
        }

        velocity = new Vector3(
            velocity.x + maxCharVelocityRun.x * (lateralAcceleration) * Time.fixedDeltaTime,
            velocity.y,
            velocity.z + maxCharVelocityRun.z * (forwardAcceleration) * Time.fixedDeltaTime
        );

        if (velocity.x > maxCharVelocityRun.x) velocity.x = maxCharVelocityRun.x;
        if (velocity.z > maxCharVelocityRun.z) velocity.z = maxCharVelocityRun.z;

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
