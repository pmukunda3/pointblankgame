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

            public Vector3 maxStepSize;

            public ClimbValidator climbValidator;

            protected Vector2 mouseInput;
            protected Vector2 moveInput;
            protected bool jumpInput = false;

            protected Vector3 groundNormal = Vector3.zero;
            protected Vector3 groundPoint = Vector3.zero;

            public new void Start() {
                base.Start();
            }

            public override void UseInput(Vector2 moveInput, Vector2 mouseInput, UserInput.Actions actions) {
                this.moveInput = moveInput;
                this.mouseInput = mouseInput;

                if (actions.jump.down) {
                    if (climbValidator.ValidateClimbAttempt()) {
                        animator.SetTrigger("TRG_climb");
                    }
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

            public override void MoveRigidbody(Vector3 localRigidbodyVelocity) {
                Quaternion movementDirection;
                if (rigidbody.velocity.sqrMagnitude < 0.0001f) {
                    movementDirection = rigidbody.rotation;
                }
                else {
                    movementDirection = Quaternion.FromToRotation(new Vector3(-rigidbody.velocity.x, 0f, rigidbody.velocity.z).normalized, Vector3.forward);
                }
                Debug.DrawRay(rigidbody.position + Vector3.up * 0.001f, movementDirection * new Vector3(0f, maxStepSize.y * 0.5f, maxStepSize.z), Color.magenta);
                Debug.DrawRay(rigidbody.position + Vector3.up * maxStepSize.y, movementDirection * Vector3.forward * maxStepSize.z, Color.magenta);
            }

            public override void CollisionEnter(Collision collision) {
                RaycastHit stepTopInfo;
                if (CheckStep(out stepTopInfo)) {
                    if (stepTopInfo.point.y > rigidbody.position.y) {
                        Debug.Log("Stepping onto this world position: " + stepTopInfo.point.ToString("F4"));
                        rigidbody.position += Vector3.up * stepTopInfo.point.y;
                    }
                }
            }

            protected bool CheckGrounded() {
                RaycastHit hitInfo;

                Debug.DrawLine(rigidbody.position + (Vector3.up * 0.1f), rigidbody.position + (Vector3.up * 0.1f) + (Vector3.down * groundCheckDistance), Color.yellow);

                if (Physics.Raycast(rigidbody.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, groundCheckDistance, player.raycastMask)) {
                    groundNormal = hitInfo.normal;
                    groundPoint = hitInfo.point;
                    return true;
                }
                else {
                    groundNormal = Vector3.zero;
                    return false;
                }
            }

            protected void StickToGroundHelper(float downwardDistance) {
                RaycastHit hitInfo;

                if (Physics.SphereCast(rigidbody.position, 0.15f, Vector3.down, out hitInfo, downwardDistance, player.raycastMask)) {
                    if (Mathf.Abs(Vector3.Angle(hitInfo.normal, Vector3.up)) < 85f) {
                        if (rigidbody.velocity.y < 0f) {
                            rigidbody.velocity = Vector3.ProjectOnPlane(rigidbody.velocity, hitInfo.normal) + Vector3.up * rigidbody.velocity.y;
                        }
                        else {
                            rigidbody.velocity = Vector3.ProjectOnPlane(rigidbody.velocity, hitInfo.normal);
                        }
                    }
                }
            }

            protected bool CheckStep(out RaycastHit stepTopInfo) {
                RaycastHit lowerCast;
                RaycastHit upperCast;

                Quaternion movementDirection;
                if (Vector3.ProjectOnPlane(rigidbody.velocity, Vector3.up).sqrMagnitude > 0.001f) {
                    movementDirection = Quaternion.FromToRotation(new Vector3(-rigidbody.velocity.x, 0f, rigidbody.velocity.z).normalized, Vector3.forward);

                    if (Physics.Raycast(rigidbody.position + Vector3.up * 0.001f, movementDirection * new Vector3(0f, maxStepSize.y * 0.5f, maxStepSize.z), out lowerCast, maxStepSize.magnitude, player.raycastMask)
                        && !Physics.Raycast(rigidbody.position + Vector3.up * maxStepSize.y, movementDirection * Vector3.forward, out upperCast, maxStepSize.z, player.raycastMask)) {
                        return Physics.Raycast(new Vector3(lowerCast.point.x, rigidbody.position.y + maxStepSize.y, lowerCast.point.z) + movementDirection * (Vector3.forward * 0.01f), Vector3.down, out stepTopInfo, maxStepSize.y, player.raycastMask);
                    }
                    else {
                        stepTopInfo = new RaycastHit();
                        return false;
                    }
                }
                else {
                    stepTopInfo = new RaycastHit();
                    return false;
                }
            }
        }
    }
}