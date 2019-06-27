using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerControl {
    namespace State {
        public class Landing : PlayerControlState {

            public float groundCheckDistance = 0.18f;
            public float maxTurnSpeed = 1.0f;

            public float moveSpeedMultiplier = 1.0f;

            private Vector2 mouseInput;
            private Vector2 moveInput;

            private Rigidbody rigidbody;

            public new void Start() {
                base.Start();
                player.RegisterState(StateId.Player.MoveModes.Air.land, this);

                rigidbody = player.GetComponent<Rigidbody>();
            }

            public override void UseInput(Vector2 moveInput, Vector2 mouseInput, UserInput.Actions actions) {
                //if (sprint) Debug.Log("Sprint Pressed");

                if (actions.walk.active) {
                    if (moveInput.sqrMagnitude > 0.3f * 0.3f) {
                        moveInput = moveInput.normalized * 0.3f;
                    }
                }

                this.moveInput = moveInput;
                this.mouseInput = mouseInput;

                float extraRotation = Mathf.Clamp(mouseInput.x, -maxTurnSpeed, maxTurnSpeed);
                rigidbody.velocity = Quaternion.AngleAxis(player.screenMouseRatio * player.mouseSensitivity * extraRotation * Time.deltaTime, Vector3.up) * rigidbody.velocity;

                if (actions.sprint.active) animator.SetBool("sprint", true);
                if (actions.secondaryFire.down) animator.SetBool("aimMode", true);
            }

            public override void AnimatorMove(Vector3 localAnimatorVelocity, Vector3 localRigidbodyVelocity) {
                Vector3 playerVelocity = animator.velocity;
                playerVelocity.y = rigidbody.velocity.y;
                rigidbody.velocity = playerVelocity;
            }

            public override void MoveRigidbody(Vector3 localRigidbodyVelocity) {
                // This is wrong,
                // TODO: Seperate hard landing from the other landing animations.
                if (player.landingAnimation == 0) {
                    animator.SetFloat("velLocalY", 0f);
                }
                else {
                    animator.SetFloat("velLocalY", 1f);
                }
            }

            public override void UpdateAnimator(Vector3 localRigidbodyVelocity) {
                animator.SetFloat("velLocalX", moveInput.x);
                animator.SetFloat("velLocalZ", moveInput.y);
            }
        }
    }
}