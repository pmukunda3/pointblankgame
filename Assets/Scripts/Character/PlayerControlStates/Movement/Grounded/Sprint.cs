using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerControl {
    namespace State {
        public class Sprint : PlayerControlState {

            public float groundCheckDistance = 0.18f;
            public float maxTurnSpeed = 30.0f;
            public float maxMouseInput = 4.0f;
            public float mouseTurnScalar = 0.25f;
            public float moveTurnScalar = 100.0f;

            public float moveSpeedMultiplier = 1.1f;

            private Vector2 mouseInput;
            private Vector2 moveInput;
            private bool jumpRb = false;

            private Rigidbody rigidbody;
            private Vector3 groundNormal = Vector3.zero;
            private Vector3 groundPoint = Vector3.zero;

            private float playerRotation = 0.0f;
            private float mouseRotation = 0.0f;

            public new void Start() {
                base.Start();
                player.RegisterState(PlayerStateId.MoveModes.Grounded.sprint, this);

                rigidbody = player.GetComponent<Rigidbody>();
            }

            public override void AnimatorMove(Vector3 localAnimatorVelocity, Vector3 localRigidbodyVelocity) {
                Vector3 playerVelocity = moveSpeedMultiplier * animator.velocity;
                playerVelocity.y = rigidbody.velocity.y;
                rigidbody.velocity = playerVelocity;
            }

            public override void MoveRigidbody(Vector3 localRigidbodyVelocity) {
                if (CheckGrounded()) {
                    float extraRotation = mouseRotation;
                    if (Mathf.Abs(moveInput.x) > player.deadzone.x) {
                        extraRotation = moveTurnScalar * Mathf.Clamp(moveInput.x, -maxTurnSpeed, maxTurnSpeed);
                    }

                    rigidbody.MoveRotation(Quaternion.AngleAxis(Mathf.Clamp(extraRotation, -maxTurnSpeed, maxTurnSpeed) * Time.fixedDeltaTime, Vector3.up) * rigidbody.rotation);

                    if (jumpRb) {
                        rigidbody.velocity += new Vector3(0f, 6f, 0f);
                        animator.SetBool("jump", true);
                        jumpRb = false;
                    }
                }
                else {
                    animator.SetBool("grounded", false);
                    animator.SetTrigger("TRI_fall");
                }
            }

            public override void UpdateAnimator(Vector3 localRigidbodyVelocity) {
                animator.SetFloat("velLocalX", moveInput.x);
                animator.SetFloat("velLocalZ", moveInput.y);
            }

            public override void UseInput(Vector2 moveInput, Vector2 mouseInput, UserInput.Actions actions) {
                //if (!sprint) Debug.Log("Sprint Released");

                this.moveInput = moveInput;
                this.mouseInput = mouseInput;

                mouseRotation = mouseTurnScalar * player.screenMouseRatio * player.mouseSensitivity * Mathf.Clamp(mouseInput.x, -maxMouseInput, maxMouseInput);

                if (!actions.sprint.active) animator.SetBool("sprint", false);
                if (actions.secondaryFire.down) animator.SetBool("aimMode", true);
                else                            animator.SetBool("aimMode", false);

                if (actions.jump.down) jumpRb = true;
            }

            private bool CheckGrounded() {
                RaycastHit hitInfo;

                Debug.DrawLine(player.transform.position + (Vector3.up * 0.1f), player.transform.position + (Vector3.up * 0.1f) + (Vector3.down * groundCheckDistance), Color.yellow);

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
        }
    }
}