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

            public float maxTimeButtonHold = 0.75f;
            public float jumpLateralInputClearingDamp = 6f;
            public float jumpForwardInputClearingDamp = 6f;

            private Vector2 mouseInput;
            private Vector2 moveInput;
            private bool jumpInput = false;

            private Rigidbody rigidbody;
            private Vector3 groundNormal = Vector3.zero;
            private Vector3 groundPoint = Vector3.zero;

            private float playerRotation = 0.0f;
            private float mouseRotation = 0.0f;

            public new void Start() {
                base.Start();
                player.RegisterState(StateId.Player.MoveModes.Grounded.sprint, this);

                rigidbody = player.GetComponent<Rigidbody>();

                EventManager.StartListening<MecanimBehaviour.SprintEvent>(new UnityEngine.Events.UnityAction(OnSprintEvent));
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

                    //if (jumpInput) {
                    //    //rigidbody.velocity += new Vector3(0f, 6f, 0f);
                    //    animator.SetBool("jump", true);
                    //    jumpInput = false;
                    //}
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
                //if (!sprint) Debug.Log("Sprint Released");

                this.moveInput = moveInput;
                this.mouseInput = mouseInput;

                mouseRotation = mouseTurnScalar * player.screenMouseRatio * player.mouseSensitivity * Mathf.Clamp(mouseInput.x, -maxMouseInput, maxMouseInput);

                if (!actions.sprint.active) animator.SetBool("sprint", false);
                if (actions.secondaryFire.down) {
                    animator.SetBool("aimMode", true);
                }

                if (!jumpInput && actions.jump.down) {
                    player.shared.timeHeldJump = new Vector3(0f, 0f, 0f);
                    jumpInput = true;
                }
                else if (actions.jump.active) {
                    if (player.shared.timeHeldJump.y < maxTimeButtonHold) {
                        player.shared.timeHeldJump.y += Time.deltaTime;

                        if (Mathf.Abs(moveInput.x) > 0.4f) {
                            player.shared.timeHeldJump.x += Time.deltaTime;
                        }
                        else {
                            player.shared.timeHeldJump.x = Mathf.Lerp(player.shared.timeHeldJump.x, 0.0f, jumpLateralInputClearingDamp * Time.deltaTime);
                        }
                        if (Mathf.Abs(moveInput.y) > 0.4f) {
                            player.shared.timeHeldJump.z += Time.deltaTime;
                        }
                        else {
                            player.shared.timeHeldJump.z = Mathf.Lerp(player.shared.timeHeldJump.x, 0.0f, jumpForwardInputClearingDamp * Time.deltaTime);
                        }
                    }
                    else {
                        Debug.Log("JUMP := true");
                        animator.SetTrigger("TRG_jump");
                    }
                }
                else if (jumpInput) {
                    animator.SetTrigger("TRG_jump");
                }
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

            private void OnSprintEvent() {
                jumpInput = false;
                player.weaponController.aimingWeapon = false;
                animator.SetBool("aimMode", false);
                animator.speed = 1.0f;

                this.moveInput = player.GetLatestMoveInput();

                player.SetState(StateId.Player.MoveModes.Grounded.sprint);
            }
        }
    }
}