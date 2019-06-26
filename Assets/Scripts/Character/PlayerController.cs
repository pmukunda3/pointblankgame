using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerControl {

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

    [RequireComponent(typeof(Rigidbody), typeof(Animator))]
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
        public IMovementState airControlFromJump;

        public WeaponController weaponController;

        private Rigidbody rigidbody;
        private Animator animator;

        private Dictionary<Id, PlayerControlState> playerControlStates;
        public PlayerControlState currPlayerState;
        private PlayerControlState emptyState;

        private bool jump = false;
        private bool jumpAllowed = true;
        private bool climbing = false;
        private bool climbingLowerTrigger = false;

        private bool grounded = true;
        private Vector3 groundNormal;
        private Vector3 groundPoint;

        private bool screenControl = true;
        private float aimPitch = 0f;

        private float speedTargetX;
        private float speedTargetY;
        private Vector2 mouseInput;

        private Vector3 localVelocity = Vector3.zero;
        private Vector2 moveInput = Vector2.zero;

        private LayerMask mask;
        public LayerMask raycastMask {
            get { return mask; }
        }

            // TODO: Create a data passing interface between these states.
        public int landingAnimation = 0;

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

        private VelocityBuffer rbVelBuffer;
        private VelocityBuffer animVelBuffer;

        public Quaternion AimDirection() {
            return Quaternion.Euler(-aimPitch, transform.eulerAngles.y, 0f);
        }

        public float AimPitch() {
            return aimPitch;
        }

        private void Start() {
            rigidbody = gameObject.GetComponent<Rigidbody>();
            animator = gameObject.GetComponent<Animator>();

            StateMachineBehaviour[] stateMachineBehaviours = animator.GetBehaviours<MecanimStateBehaviourEvents>();

            runningState = gameObject.GetComponent<Running>() as IMovementState;
            airControlFromJump = gameObject.GetComponent<AirControlFromJump>() as IMovementState;

            EventManager.StartListening<WeaponFirePrimary>(
                new UnityEngine.Events.UnityAction(WeaponFirePrimaryCallbackTest));
            EventManager.StartListening<WeaponFireSecondary, float>(
                new UnityEngine.Events.UnityAction<float>(WeaponFirePrimaryCallbackTest));

            Cursor.lockState = CursorLockMode.Locked;
            screenControl = true;

            mask = LayerMask.GetMask("Static Level Geometry", "Moving Level Geometry");
            rbVelBuffer = new VelocityBuffer(16);
            animVelBuffer = new VelocityBuffer(16);

            playerControlStates = new Dictionary<Id, PlayerControlState>();

            currPlayerState = emptyState = gameObject.AddComponent<EmptyPlayerState>() as PlayerControlState;
            RegisterState(PlayerStateId.empty, emptyState);
        }

        private void WeaponFirePrimaryCallbackTest() {
            Debug.Log("Fire Weapon Primary");
        }

        private void WeaponFirePrimaryCallbackTest(float holdTime) {
            Debug.Log("Fire Weapon Secondary: " + holdTime);
        }

        private void Update() {
            bool walk   = Input.GetKey(KeyCode.X);
            bool sprint = Input.GetKey(KeyCode.LeftShift);
            bool crouch = Input.GetKey(KeyCode.LeftControl);
            bool use    = Input.GetKey(KeyCode.F);
            bool primaryFire = Input.GetKey(KeyCode.Mouse0);

                //TODO: Create a proper way to pass in input
            bool jump = Input.GetKeyDown(KeyCode.Space);
            bool secondaryFire = Input.GetKeyDown(KeyCode.Mouse1);

            speedTargetX = Input.GetAxis("Horizontal");
            speedTargetY = Input.GetAxis("Vertical");

            //speedTargetX = speedTargetY = 0f;
            //if (Input.GetKey(KeyCode.W)) speedTargetY += 1.0f;
            //if (Input.GetKey(KeyCode.A)) speedTargetX -= 1.0f;
            //if (Input.GetKey(KeyCode.S)) speedTargetY -= 1.0f;
            //if (Input.GetKey(KeyCode.D)) speedTargetX += 1.0f;

            moveInput = new Vector2(speedTargetX, speedTargetY);
            if (moveInput.sqrMagnitude > 1.0f) {
                moveInput.Normalize();
            }

            if (screenControl) {
                mouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

                aimPitch += mouseSensitivity * mouseInput.y * Time.deltaTime;
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
                mouseInput = Vector2.zero;
            }

            if (Input.GetKeyDown(KeyCode.Mouse0) && !screenControl) {
                Cursor.lockState = CursorLockMode.Locked;
                screenControl = true;
            }

            if (Input.GetKeyDown(KeyCode.Keypad1)) rigidbody.position = new Vector3(-32, 0, 22);
            if (Input.GetKeyDown(KeyCode.Keypad2)) rigidbody.position = new Vector3(-12, 4, 40);
            if (Input.GetKeyDown(KeyCode.Keypad3)) rigidbody.position = new Vector3(-12, 0, 32);
            if (Input.GetKeyDown(KeyCode.Keypad4)) rigidbody.position = new Vector3(-12, 8, 32);

            currPlayerState.UseInput(moveInput, mouseInput, walk, sprint, crouch, jump, use, primaryFire, secondaryFire);
        }

        private void FixedUpdate() {

            Vector3 localRigidbodyVelocity = Quaternion.Inverse(rigidbody.rotation) * Vector3.ProjectOnPlane(rigidbody.velocity, Vector3.up);
            Debug.DrawRay(rigidbody.position + 0.5f * Vector3.up, Quaternion.Inverse(rigidbody.rotation) * rigidbody.velocity, Color.green);

            float rotationAmount = Mathf.Atan2(moveInput.x, moveInput.y);
            float forwardAmount = moveInput.y;

            currPlayerState.MoveRigidbody(localRigidbodyVelocity);

            localRigidbodyVelocity = Quaternion.Inverse(rigidbody.rotation) * Vector3.ProjectOnPlane(rigidbody.velocity, Vector3.up);
            UpdateAnimator(localRigidbodyVelocity);

            rbVelBuffer.AddVelocity(rigidbody.velocity);
        }

        public void OnAnimatorMove() {
            //Vector3 localAnimatorVelocity = Quaternion.Inverse(animator.rootRotation) * Vector3.ProjectOnPlane(animator.velocity, Vector3.up);
            //Vector3 localRigidbodyVelocity = Quaternion.Inverse(rigidbody.rotation) * Vector3.ProjectOnPlane(rigidbody.velocity, Vector3.up);
            Vector3 localAnimatorVelocity = Quaternion.Inverse(animator.rootRotation) * animator.velocity;
            Vector3 localRigidbodyVelocity = Quaternion.Inverse(rigidbody.rotation) * rigidbody.velocity;
            currPlayerState.AnimatorMove(localAnimatorVelocity, localRigidbodyVelocity);

            animVelBuffer.AddVelocity(animator.velocity);
        }

        private void UpdateAnimator(Vector3 localVelocity) {
            currPlayerState.UpdateAnimator(localVelocity);
        }

        public void SetState(Id stateId) {
            PlayerControlState state;
            if (playerControlStates.TryGetValue(stateId, out state)) {
                currPlayerState = state;
            }
            else {
                currPlayerState = emptyState;
            }
        }

        public void RegisterState(Id stateId, PlayerControlState state) {
            playerControlStates.Add(stateId, state);
        }

        private class EmptyPlayerState : PlayerControlState {
            public override void AnimatorMove(Vector3 localAnimatorVelocity, Vector3 localRigidbodyVelocity) {
                //Debug.Log("Empty State: AnimatorMove");
            }

            public override void MoveRigidbody(Vector3 localRigidbodyVelocity) {
                //Debug.Log("Empty State: MoveRigidbody");
            }

            public override void UpdateAnimator(Vector3 localRigidbodyVelocity) {
                //Debug.Log("Empty State: UpdateAnimator");
            }

            public override void UseInput(Vector2 moveInput, Vector2 mouseInput, bool walk, bool sprint, bool crouch, bool jump, bool use, bool primaryFire, bool secondaryFire) {
                //Debug.Log("Empty State: UseInput");
            }
        }
    }
}