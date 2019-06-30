using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerControl {
    namespace State {
        public abstract class Grounded : PlayerControlState {

            public float groundCheckDistance = 0.18f;
            public float maxTurnSpeed = 120f;

            public float moveSpeedMultiplier = 1.0f;

            public float maxTimeButtonHold = 0.5f;
            public float jumpLateralInputClearingDamp = 12f;
            public float jumpForwardInputClearingDamp = 12f;

            protected Vector2 mouseInput;
            protected Vector2 moveInput;
            protected bool jumpInput = false;

            protected new Rigidbody rigidbody;
            protected Vector3 groundNormal = Vector3.zero;
            protected Vector3 groundPoint = Vector3.zero;

            public new void Start() {
                base.Start();

                rigidbody = player.GetComponent<Rigidbody>();
            }

            public override void UseInput(Vector2 moveInput, Vector2 mouseInput, UserInput.Actions actions) {
                this.moveInput = moveInput;
                this.mouseInput = mouseInput;

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

            protected bool CheckGrounded() {
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