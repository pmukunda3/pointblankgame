using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerControl {
    namespace State {
        public class Aiming : PlayerControlState {

            public float groundCheckDistance = 0.18f;
            public float maxTurnSpeed = 1.0f;

            public float moveSpeedMultiplier = 1.0f;

            private Vector2 mouseInput;
            private Vector2 moveInput;
            private bool jumpRb = false;

            private Rigidbody rigidbody;
            private Vector3 groundNormal = Vector3.zero;
            private Vector3 groundPoint = Vector3.zero;

            private Vector3 velocityReset = new Vector3(0f, 1f, 0f);

            public new void Start() {
                base.Start();
                player.RegisterState(PlayerStateId.MoveModes.Grounded.aiming, this);

                rigidbody = player.GetComponent<Rigidbody>();
            }

            public override void UseInput(Vector2 moveInput, Vector2 mouseInput, bool walk, bool sprint, bool crouch, bool jump, bool use, bool primaryFire, bool secondaryFire) {
                //if (sprint) Debug.Log("Sprint Pressed");

                if (walk) {
                    if (moveInput.sqrMagnitude > 0.3f * 0.3f) {
                        moveInput = moveInput.normalized * 0.3f;
                    }
                }

                //Debug.Log(moveInput.ToString("F3"));

                this.moveInput = moveInput;
                this.mouseInput = mouseInput;

                float extraRotation = Mathf.Clamp(mouseInput.x, -maxTurnSpeed, maxTurnSpeed);
                rigidbody.velocity = Quaternion.AngleAxis(player.screenMouseRatio * player.mouseSensitivity * extraRotation * Time.deltaTime, Vector3.up) * rigidbody.velocity;

                if (sprint) animator.SetBool("sprint", true);
                else animator.SetBool("sprint", false);

                if (secondaryFire) {
                    animator.SetBool("aimMode", false);
                    Debug.Log("Secondary Fire pressed in AIMING MODE");
                }

                if (jump) jumpRb = true;
            }

            public override void AnimatorMove(Vector3 localAnimatorVelocity, Vector3 localRigidbodyVelocity) {
                //rigidbody.position = animator.rootPosition;
                //player.transform.rotation = animator.rootRotation;

                Vector3 playerVelocity = moveSpeedMultiplier * animator.velocity;

                playerVelocity.y = rigidbody.velocity.y;
                rigidbody.velocity = playerVelocity;
            }

            public override void MoveRigidbody(Vector3 localRigidbodyVelocity) {
                if (CheckGrounded()) {
                    //rigidbody.velocity = Vector3.Scale(rigidbody.velocity, velocityReset);
                    rigidbody.MoveRotation(Quaternion.AngleAxis(player.screenMouseRatio * player.mouseSensitivity * mouseInput.x * Time.fixedDeltaTime, Vector3.up) * rigidbody.rotation);
                    if (jumpRb) {
                        Debug.Log("jumpRb");
                        rigidbody.velocity += new Vector3(0f, 4f, 0f);
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