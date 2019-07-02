using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerControl {
    namespace State {
        public class Sprint : Grounded {

            public float mouseTurnScalar = 0.25f;
            public float moveTurnScalar = 100.0f;
            public float maxInputTurnSpeed = 0.75f;

            public AnimationCurve realignTurnSpeed;

            public new void Start() {
                base.Start();
                player.RegisterState(StateId.Player.MoveModes.Grounded.sprint, this);

                EventManager.StartListening<MecanimBehaviour.SprintEvent>(new UnityEngine.Events.UnityAction(OnSprintEvent));
            }

            public override void AnimatorMove(Vector3 localAnimatorVelocity, Vector3 localRigidbodyVelocity) {
                Vector3 playerVelocity = moveSpeedMultiplier * animator.velocity;
                playerVelocity.y = rigidbody.velocity.y;
                rigidbody.velocity = playerVelocity;
            }

            public override void AnimatorIK() {
                // do nothing
            }

            public override void MoveRigidbody(Vector3 localRigidbodyVelocity) {
                base.MoveRigidbody(localRigidbodyVelocity);

                if (CheckGrounded()) {
                    float extraRotation = 0.0f;
                    if (Mathf.Abs(moveInput.x) > player.deadzone.x) {
                        extraRotation = moveTurnScalar * Mathf.Clamp(moveInput.x, -maxInputTurnSpeed, maxInputTurnSpeed);
                    }
                    float angleDiff = player.LookToMoveAngle();
                    if (Mathf.Abs(angleDiff) > 1.0f) {
                        extraRotation += Mathf.Sign(-angleDiff) * realignTurnSpeed.Evaluate((Mathf.Abs(angleDiff) / Time.fixedDeltaTime) / maxTurnSpeed) * maxTurnSpeed;
                    }
                    if (extraRotation != 0.0f) {
                        rigidbody.MoveRotation(Quaternion.AngleAxis(Mathf.Clamp(extraRotation, -maxTurnSpeed, maxTurnSpeed) * Time.fixedDeltaTime, Vector3.up) * rigidbody.rotation);
                    }
                    StickToGroundHelper(0.35f);
                }
                else {
                    animator.SetBool("grounded", false);
                    animator.SetTrigger("TRG_fall");
                }
            }

            public override void UpdateAnimator(Vector3 localRigidbodyVelocity) {
                animator.SetFloat("velLocalX", moveInput.x);
                animator.SetFloat("velLocalZ", moveInput.y);
            }

            public override void UseInput(Vector2 moveInput, Vector2 mouseInput, UserInput.Actions actions) {
                base.UseInput(moveInput, mouseInput, actions);

                if (!actions.sprint.active) animator.SetBool("sprint", false);
                if (actions.secondaryFire.down) animator.SetBool("aimMode", true);
            }

            private void OnSprintEvent() {
                jumpInput = false;
                animator.SetBool("aimMode", false);
                animator.speed = 1.0f;

                this.moveInput = player.GetLatestMoveInput();

                player.SetState(StateId.Player.MoveModes.Grounded.sprint);
            }
        }
    }
}