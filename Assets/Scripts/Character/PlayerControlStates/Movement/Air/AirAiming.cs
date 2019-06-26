using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerControl {
    namespace State {
        public class AirAiming : PlayerControlState {

            public float groundCheckDistance = 0.18f;
            public float maxTurnSpeed = 0.2f;

            private Vector2 mouseInput;
            private Vector2 moveInput;

            private Rigidbody rigidbody;
            private Vector3 groundNormal = Vector3.zero;
            private Vector3 groundPoint = Vector3.zero;

            public new void Start() {
                base.Start();
                player.RegisterState(PlayerStateId.MoveModes.Air.aiming, this);

                rigidbody = player.GetComponent<Rigidbody>();
            }

            public override void AnimatorMove(Vector3 localAnimatorVelocity, Vector3 localRigidbodyVelocity) {
                //MovementChange moveChange = freeRoamMovement.CalculateAcceleration(moveInput, localRigidbodyVelocity, Time.fixedDeltaTime);

                //Vector3 newVelocity = (animator.deltaPosition * moveSpeedMultiplier) / Time.deltaTime;
                //newVelocity += Vector3.Scale(moveChange.localAcceleration, new Vector3(1f, 0f, 0f)) * Time.deltaTime;
                //newVelocity.y = rigidbody.velocity.y;
                //rigidbody.velocity = newVelocity;

                rigidbody.velocity = animator.velocity;
            }

            public override void MoveRigidbody(Vector3 localRigidbodyVelocity) {
                if (!CheckGrounded()) {
                    //animator.SetBool("grounded", false);
                    Debug.Log("SetBool('grounded', false);");
                }

                //MovementChange moveChange = runningState.CalculateAcceleration(moveInput, localRigidbodyVelocity, Time.fixedDeltaTime);

                //if (jump) {
                //    moveChange.localVelocityOverride.y = 4.0f;
                //    jumpAllowed = false;
                //}

            }

            public override void UpdateAnimator(Vector3 localRigidbodyVelocity) {
                animator.SetFloat("velLocalX", moveInput.x);
                animator.SetFloat("velLocalZ", moveInput.y);
            }

            public override void UseInput(Vector2 moveInput, Vector2 mouseInput, bool walk, bool sprint, bool crouch, bool jump, bool use, bool primaryFire, bool secondaryFire) {
                //if (!sprint) Debug.Log("Sprint Released");

                this.moveInput = moveInput;
                this.mouseInput = mouseInput;

                float extraRotation = Mathf.Clamp(mouseInput.x, -maxTurnSpeed, maxTurnSpeed);
                rigidbody.velocity = Quaternion.AngleAxis(0.25f * player.screenMouseRatio * player.mouseSensitivity * extraRotation * Time.deltaTime, Vector3.up) * rigidbody.velocity;

                if (!sprint) animator.SetBool("sprint", false);
                if (secondaryFire) animator.SetBool("aimMode", true);
                else animator.SetBool("aimMode", false);
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