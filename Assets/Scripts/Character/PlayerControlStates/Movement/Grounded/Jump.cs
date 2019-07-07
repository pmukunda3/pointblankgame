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

            public float maxTimeButtonHold = 0.4f;

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
            private bool leavingGround = false;

            private Vector3 groundNormal = Vector3.zero;
            private Vector3 groundPoint = Vector3.zero;

            private float worldPositionY = 0.0f;

            public new void Start() {
                base.Start();
                player.RegisterState(StateId.Player.MoveModes.Grounded.jump, this);

                EventManager.StartListening<MecanimBehaviour.JumpEvent>(new UnityEngine.Events.UnityAction(OnJumpEvent));
            }

            public override void AnimatorMove(Vector3 localAnimatorVelocity, Vector3 localRigidbodyVelocity) {
                // do nothing
            }

            public override void AnimatorIK() {
                // do nothing
            }

            public override void MoveRigidbody(Vector3 localRigidbodyVelocity) {
                if (CheckGrounded()) {
                    if (jumpInput) {
                        animator.SetFloat("jumpSpeed", forwardSpeedToJumpAnimation.Evaluate(localRigidbodyVelocity.z));

                        worldPositionY = rigidbody.position.y;
                        Debug.Log(localRigidbodyVelocity.ToString("F4") + " + " + CalculateJumpVelocity(localRigidbodyVelocity).ToString("F4"));
                        rigidbody.velocity += rigidbody.rotation * CalculateJumpVelocity(localRigidbodyVelocity);
                        jumpInput = false;
                        leavingGround = true;
                        animator.SetBool("grounded", true);
                    }
                    else if (!leavingGround) {
                        animator.SetBool("grounded", true);
                    }
                }
                else if (CheckGroundUnderJump()) {
                    leavingGround = false;
                    animator.SetBool("jumpGroundCheck", true);
                    animator.SetBool("grounded", false);
                }
                else {
                    animator.SetBool("jumpGroundCheck", false);
                    animator.SetBool("grounded", false);
                }

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

                if (Physics.Raycast(player.transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, groundCheckDistance, player.raycastMask)) {
                    groundNormal = hitInfo.normal;
                    groundPoint = hitInfo.point;
                    return true;
                }
                else {
                    groundNormal = Vector3.zero;
                    return false;
                }
            }

            private bool CheckGroundUnderJump() {
                RaycastHit hitInfo;

                if (rigidbody.position.y > worldPositionY - groundUnderEpsilon) {
                    float groundUnderCheckDistance = groundCheckDistance + rigidbody.position.y - worldPositionY + groundUnderEpsilon;
                    Debug.DrawLine(rigidbody.position + (Vector3.up * 0.1f), rigidbody.position + (Vector3.up * 0.1f) + (Vector3.down * groundUnderCheckDistance), Color.red);

                    return Physics.Raycast(player.transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, groundUnderCheckDistance, player.raycastMask);
                }
                else {
                    return false;
                }
            }

            private Vector3 CalculateJumpVelocity(Vector3 localRigidbodyVelocity) {
                return new Vector3(
                    Mathf.Sign(this.moveInput.x) * lateralSpeedTimeHeld.Evaluate(player.shared.timeHeldJump.x) * lateralSpeedCharSpeed.Evaluate(Mathf.Abs(localRigidbodyVelocity.z)),
                    verticalSpeedTimeHeld.Evaluate(player.shared.timeHeldJump.y) * verticalSpeedCharSpeed.Evaluate(Mathf.Abs(localRigidbodyVelocity.z)),
                    Mathf.Sign(this.moveInput.y) * forwardSpeedTimeHeld.Evaluate(player.shared.timeHeldJump.z) * forwardSpeedCharSpeed.Evaluate(Mathf.Abs(localRigidbodyVelocity.z))
                );
            }

            private void OnJumpEvent() {
                jumpInput = true;
                animator.speed = 1.0f;
                player.legsCollider.enabled = true;
                this.moveInput = player.GetLatestMoveInput();

                player.SetState(StateId.Player.MoveModes.Grounded.jump);
            }
        }
    }
}