using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerControl {
    namespace State {
        public class Jump : PlayerControlState {

            public float groundCheckDistance = 0.18f;
            public float groundUnderEpsilon = 0.005f;
            public float maxTurnSpeed = 30.0f;
            public float maxMouseInput = 4.0f;
            public float mouseTurnScalar = 0.25f;
            public float moveTurnScalar = 100.0f;

            public float maxTimeButtonHold = 12.0f / 60.0f;
            public float maxTimeUngrounded = 3.0f / 60.0f;

            public float jumpLateralInputClearingDamp = 12f;
            public float jumpForwardInputClearingDamp = 12f;

            public AnimationCurve forwardSpeedToJumpAnimation;

            public AnimationCurve lateralSpeedTimeHeld;
            public AnimationCurve lateralSpeedCharSpeed;
            public AnimationCurve forwardSpeedTimeHeld;
            public AnimationCurve forwardSpeedCharSpeed;
            public AnimationCurve verticalSpeedTimeHeld;
            public AnimationCurve verticalSpeedCharSpeed;

            public ClimbValidator climbValidator;

            private Vector2 mouseInput;
            private Vector2 moveInput;
            private bool jumpInput = true;
            private bool initiateJump = false;
            private float timeJumpInput = 0.0f;

            private bool leavingGround = false;
            private float timeUngrounded = 0.0f;

            private IMovementState airControlMovement;
            private IJumpVerticalPush jumpVerticalPush;

            private Vector3 groundNormal = Vector3.zero;
            private Vector3 groundPoint = Vector3.zero;

            private float worldPositionY = 0.0f;

            public new void Start() {
                base.Start();
                player.RegisterState(StateId.Player.MoveModes.Grounded.jump, this);

                airControlMovement = gameObject.GetComponentInChildren<AirControlFromJump>() as IMovementState;
                jumpVerticalPush = gameObject.GetComponentInChildren<AirControlFromJump>() as IJumpVerticalPush;

                EventManager.StartListening<MecanimBehaviour.JumpEvent>(new UnityEngine.Events.UnityAction(OnJumpEvent));
            }

            public override void AnimatorMove(Vector3 localAnimatorVelocity, Vector3 localRigidbodyVelocity) {
                // do nothing
            }

            public override void AnimatorIK() {
                // do nothing
            }

            public override void MoveRigidbody(Vector3 localRigidbodyVelocity) {

                if (initiateJump) {
                    initiateJump = false;
                    animator.SetFloat("jumpSpeed", forwardSpeedToJumpAnimation.Evaluate(localRigidbodyVelocity.z));

                    worldPositionY = rigidbody.position.y;
                    Debug.Log(localRigidbodyVelocity.ToString("F4") + " + " + CalculateJumpVelocity(localRigidbodyVelocity).ToString("F4"));
                    rigidbody.velocity += rigidbody.rotation * CalculateJumpVelocity(localRigidbodyVelocity);
                    animator.SetBool("grounded", true);
                    animator.SetBool("jumpGroundCheck", true);
                }

                if (jumpInput) {
                    rigidbody.AddForce(jumpVerticalPush.VerticalVelocityPush(timeJumpInput) * Vector3.up, ForceMode.Acceleration);

                    if (timeJumpInput > maxTimeButtonHold) {
                        jumpInput = false;
                        animator.SetBool("jump", false);
                    }
                }

                if (CheckGrounded()) {
                    animator.SetBool("grounded", true);
                    player.Grounded();
                }
                else {
                    if (CheckGroundUnderJump(localRigidbodyVelocity)) {
                        animator.SetBool("jumpGroundCheck", true);
                        animator.SetBool("grounded", false);
                    }
                    else {
                        animator.SetBool("jumpGroundCheck", false);
                        animator.SetBool("grounded", false);
                    }
                }

                timeJumpInput += Time.fixedDeltaTime;

                player.shared.lastRigidbodyVelocity = rigidbody.velocity;
                if (player.shared.lastRigidbodyVelocity.y > -4f) {
                    animator.SetInteger("landMode", 0);
                }
                else if (player.shared.lastRigidbodyVelocity.y > -10f) {
                    animator.SetInteger("landMode", 1);
                }
                else {
                    animator.SetInteger("landMode", 2);
                }

                MovementChange moveChange = airControlMovement.CalculateAcceleration(moveInput, localRigidbodyVelocity, Time.fixedDeltaTime);
                rigidbody.AddRelativeForce(moveChange.localAcceleration, ForceMode.Acceleration);

                //rigidbody.MoveRotation(Quaternion.AngleAxis(Mathf.Clamp(mouseRotation, -maxTurnSpeed, maxTurnSpeed) * Time.fixedDeltaTime, Vector3.up) * rigidbody.rotation);

                float angleDiff = Quaternion.Angle(rigidbody.rotation, player.AimYawQuaternion());
                if (angleDiff / Time.fixedDeltaTime > maxTurnSpeed) {
                    rigidbody.MoveRotation(Quaternion.Slerp(rigidbody.rotation, player.AimYawQuaternion(), maxTurnSpeed / angleDiff * Time.fixedDeltaTime));
                }
                else {
                    rigidbody.MoveRotation(player.AimYawQuaternion()); // same as Slerp(rb.rot, play.yawQuat(), 1.0)
                }
            }

            public override void UpdateAnimator(Vector3 localRigidbodyVelocity) {
                animator.SetFloat("velLocalX", moveInput.x);
                animator.SetFloat("velLocalZ", moveInput.y);
            }

            public override void UseInput(Vector2 moveInput, Vector2 mouseInput, UserInput.Actions actions) {
                this.moveInput = moveInput;

                if (!actions.jump.active) {
                    jumpInput = false;
                    animator.SetBool("jump", false);
                }

                if (actions.climbUp.down) {
                    if (!climbValidator.ClimbValid() && climbValidator.ValidateClimbAttempt()) {
                        animator.SetInteger("climbAnim", (int)climbValidator.GetClimbAnimation() - 1);
                        animator.SetTrigger("TRG_climb");
                    }
                }
            }

            public override void CollisionEnter(Collision collision) {
                // do nothing
            }

            private bool CheckGrounded() {
                RaycastHit hitInfo;

                Debug.DrawLine(rigidbody.position + (Vector3.up * 0.1f), rigidbody.position + (Vector3.up * 0.1f) + (Vector3.down * groundCheckDistance), Color.yellow);

                if (Physics.Raycast(player.transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, groundCheckDistance, player.raycastMask, QueryTriggerInteraction.Ignore)) {
                    groundNormal = hitInfo.normal;
                    groundPoint = hitInfo.point;
                    return true;
                }
                else {
                    groundNormal = Vector3.zero;
                    return false;
                }
            }

            private bool CheckGroundUnderJump(Vector3 localRigidbodyVelocity) {
                RaycastHit underCharHitInfo;
                RaycastHit forwardCharHitInfo;

                if (rigidbody.position.y > worldPositionY - groundUnderEpsilon) {
                    float groundUnderCheckDistance = groundCheckDistance + rigidbody.position.y - worldPositionY + groundUnderEpsilon;
                    Debug.DrawLine(rigidbody.position + (Vector3.up * 0.1f), rigidbody.position + (Vector3.up * 0.1f) + (Vector3.down * groundUnderCheckDistance), Color.red);

                    bool underChar = Physics.Raycast(rigidbody.position + (Vector3.up * 0.1f), Vector3.down, out underCharHitInfo, groundUnderCheckDistance, player.raycastMask, QueryTriggerInteraction.Ignore);

                    Vector3 endDirection = rigidbody.rotation * new Vector3(0f, -groundUnderCheckDistance, localRigidbodyVelocity.z * CalculateT(rigidbody.position.y, worldPositionY, rigidbody.velocity.y, Physics.gravity.y));
                    Debug.DrawLine(rigidbody.position + (Vector3.up * 0.1f), rigidbody.position + (Vector3.up * 0.1f) + endDirection, Color.red);

                    bool forwardChar = Physics.SphereCast(rigidbody.position + (Vector3.up * 0.1f), 0.3f, endDirection, out forwardCharHitInfo, endDirection.magnitude, player.raycastMask, QueryTriggerInteraction.Ignore);
                    if (forwardChar) {
                        Debug.DrawRay(forwardCharHitInfo.point, forwardCharHitInfo.normal * 0.1f, Color.blue, 2f);
                    }
                    return underChar || forwardChar;
                }
                else {
                    return false;
                }
            }

            private float CalculateT(float currPos, float targetPos, float verticalSpeed, float accel) {
                float x = currPos - targetPos;
                float t0 = (-verticalSpeed + Mathf.Sqrt((verticalSpeed * verticalSpeed) - 2.0f * accel * x)) / accel;
                float t1 = (-verticalSpeed - Mathf.Sqrt((verticalSpeed * verticalSpeed) - 2.0f * accel * x)) / accel;

                if (t0 > 0) {
                    if (t1 > 0) {
                        return Mathf.Min(t0, t1);
                    }
                    else {
                        return t0;
                    }
                }
                else {
                    if (t1 > 0) {
                        return t1;
                    }
                    else {
                        return 0.0f;
                    }
                }
            }

            private Vector3 CalculateJumpVelocity(Vector3 localRigidbodyVelocity) {
                Debug.LogFormat("Time held x={0}, y={1}, z={2}", player.shared.timeHeldJump.x, player.shared.timeHeldJump.y, player.shared.timeHeldJump.z);
                return new Vector3(
                    Mathf.Sign(this.moveInput.x) * lateralSpeedTimeHeld.Evaluate(player.shared.timeHeldJump.x) * lateralSpeedCharSpeed.Evaluate(Mathf.Abs(localRigidbodyVelocity.z)),
                    verticalSpeedTimeHeld.Evaluate(player.shared.timeHeldJump.y) * verticalSpeedCharSpeed.Evaluate(Mathf.Abs(localRigidbodyVelocity.z)),
                    Mathf.Sign(this.moveInput.y) * forwardSpeedTimeHeld.Evaluate(player.shared.timeHeldJump.z) * forwardSpeedCharSpeed.Evaluate(Mathf.Abs(localRigidbodyVelocity.z))
                );
            }

            private void OnJumpEvent() {
                jumpInput = true;
                animator.SetBool("jump", true);
                initiateJump = true;
                timeJumpInput = 0.0f;
                timeUngrounded = 0.0f;
                animator.speed = 1.0f;
                player.legsCollider.enabled = true;
                this.moveInput = player.GetLatestMoveInput();

                player.SetState(StateId.Player.MoveModes.Grounded.jump);
            }
        }
    }
}