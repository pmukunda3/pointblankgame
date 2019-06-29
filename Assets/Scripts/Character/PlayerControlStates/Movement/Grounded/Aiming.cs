using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerControl {
    namespace State {
        public class Aiming : PlayerControlState {

            public float groundCheckDistance = 0.18f;
            public float maxTurnSpeed = 1.0f;

            public float moveSpeedMultiplier = 1.0f;

            public float maxTimeButtonHold = 0.25f;
            public float jumpLateralInputClearingDamp = 18f;
            public float jumpForwardInputClearingDamp = 18f;

            private Vector2 mouseInput;
            private Vector2 moveInput;
            private bool jumpInput = false;

            private Rigidbody rigidbody;
            private Vector3 groundNormal = Vector3.zero;
            private Vector3 groundPoint = Vector3.zero;

            private Vector3 velocityReset = new Vector3(0f, 1f, 0f);

            public new void Start() {
                base.Start();
                player.RegisterState(StateId.Player.MoveModes.Grounded.aiming, this);

                rigidbody = player.GetComponent<Rigidbody>();

                EventManager.StartListening<MecanimBehaviour.AimingEvent>(new UnityEngine.Events.UnityAction(TestAimingEvent));
            }

            public override void UseInput(Vector2 moveInput, Vector2 mouseInput, UserInput.Actions actions) {
                //if (sprint) Debug.Log("Sprint Pressed");

                if (actions.walk.active) {
                    if (moveInput.sqrMagnitude > 0.3f * 0.3f) {
                        moveInput = moveInput.normalized * 0.3f;
                    }
                }

                //Debug.Log(moveInput.ToString("F3"));

                this.moveInput = moveInput;
                this.mouseInput = mouseInput;

                float extraRotation = Mathf.Clamp(mouseInput.x, -maxTurnSpeed, maxTurnSpeed);
                rigidbody.velocity = Quaternion.AngleAxis(player.screenMouseRatio * player.mouseSensitivity * extraRotation * Time.deltaTime, Vector3.up) * rigidbody.velocity;

                if (actions.sprint.down) animator.SetBool("sprint", true);
                if (actions.secondaryFire.down) animator.SetBool("aimMode", false);

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
                    //if (jumpInput) {
                    //    Debug.Log("jumpRb");
                    //    //rigidbody.velocity += new Vector3(0f, 3f, 0f);
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

            private void TestAimingEvent() {
                jumpInput = false;
                player.weaponController.aimingWeapon = true;
                animator.SetBool("sprint", false);
                animator.speed = 1.0f;

                this.moveInput = player.GetLatestMoveInput();

                player.SetState(StateId.Player.MoveModes.Grounded.aiming);
            }
        }
    }
}