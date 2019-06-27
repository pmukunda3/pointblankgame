using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerControl {
    namespace State {
        public class FreeRoam : PlayerControlState {

            public float groundCheckDistance = 0.18f;
            public float maxTurnSpeed = 1.0f;

            public float moveSpeedMultiplier = 1.0f;

            private IMovementState freeRoamMovement;

            private Vector2 mouseInput;
            private Vector2 moveInput;
            private bool jumpRb = false;

            private Rigidbody rigidbody;
            private Vector3 groundNormal = Vector3.zero;
            private Vector3 groundPoint = Vector3.zero;

            private float lateralSpeed = 0f;
            private Vector3 direction = Vector3.zero;

            public new void Start() {
                base.Start();
                player.RegisterState(StateId.Player.MoveModes.Grounded.freeRoam, this);

                rigidbody = player.GetComponent<Rigidbody>();
                freeRoamMovement = gameObject.GetComponentInChildren<Running>() as IMovementState;

                EventManager.StartListening<MecanimBehaviour.FreeRoamEvent>(new UnityEngine.Events.UnityAction(TestFreeRoamEvent));
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

                if (actions.jump.down) jumpRb = true;
            }

            public override void AnimatorMove(Vector3 localAnimatorVelocity, Vector3 localRigidbodyVelocity) {
                MovementChange moveChange = freeRoamMovement.CalculateAcceleration(moveInput, localRigidbodyVelocity, Time.deltaTime);

                Vector3 forwardVector;
                if (localRigidbodyVelocity.sqrMagnitude > 1.0f && localAnimatorVelocity.sqrMagnitude > 1.0f
                    && Vector3.Dot(localAnimatorVelocity.normalized, localRigidbodyVelocity.normalized) > 0.8f) {
                    forwardVector = Vector3.Project(localAnimatorVelocity, localRigidbodyVelocity);
                }
                else {
                    forwardVector = Vector3.Project(localAnimatorVelocity, Vector3.forward);
                }

                Vector3 newVelocity = forwardVector;
                newVelocity.y = localRigidbodyVelocity.y;
                newVelocity.x = localRigidbodyVelocity.x + moveChange.localAcceleration.x * Time.deltaTime;

                rigidbody.velocity = rigidbody.rotation * newVelocity;

                Debug.DrawRay(rigidbody.position + 0.7f * Vector3.up, localRigidbodyVelocity, Color.blue);
                Debug.DrawRay(rigidbody.position + 0.8f * Vector3.up, localAnimatorVelocity, Color.cyan);
                //Debug.Log("input: " + moveInput.ToString("F2") + ", local Rb Vel: " + localRigidbodyVelocity.ToString("F3") + ", local Accel: " + moveChange.localAcceleration.ToString("F3") + ", new Vel: " + newVelocity.ToString("F3"));

                Debug.DrawRay(rigidbody.position + 0.5f * Vector3.up, rigidbody.velocity, Color.white);
                Debug.DrawRay(rigidbody.position + 0.6f * Vector3.up, animator.velocity, Color.red);
            }

            public override void MoveRigidbody(Vector3 localRigidbodyVelocity) {
                if (CheckGrounded()) {
                    rigidbody.MoveRotation(Quaternion.AngleAxis(player.screenMouseRatio * player.mouseSensitivity * mouseInput.x * Time.fixedDeltaTime, Vector3.up) * rigidbody.rotation);

                    //MovementChange moveChange = freeRoamMovement.CalculateAcceleration(moveInput, localRigidbodyVelocity, Time.fixedDeltaTime);

                    //if (moveChange.localVelocityOverride == localRigidbodyVelocity) {
                    //    //rigidbody.AddRelativeForce(moveChange.localAcceleration, ForceMode.Acceleration);
                    //    // or
                    //    rigidbody.AddForce(rigidbody.rotation * moveChange.localAcceleration, ForceMode.Acceleration);

                    //    rigidbody.MoveRotation(Quaternion.AngleAxis(player.screenMouseRatio * player.mouseSensitivity * mouseInput.x * Time.fixedDeltaTime, Vector3.up) * rigidbody.rotation);
                    //}
                    //else {
                    //    Vector3 localVelocityOverride = new Vector3(localRigidbodyVelocity.x, localRigidbodyVelocity.y, localRigidbodyVelocity.z);

                    //    localVelocityOverride += moveChange.localAcceleration * Time.fixedDeltaTime;

                    //    if (moveChange.localVelocityOverride.x != localRigidbodyVelocity.x) {
                    //        localVelocityOverride.x = moveChange.localVelocityOverride.x;
                    //    }
                    //    if (moveChange.localVelocityOverride.y != localRigidbodyVelocity.y) {
                    //        localVelocityOverride.y = moveChange.localVelocityOverride.y;
                    //    }
                    //    if (moveChange.localVelocityOverride.z != localRigidbodyVelocity.z) {
                    //        localVelocityOverride.z = moveChange.localVelocityOverride.z;
                    //    }

                    //    rigidbody.velocity = rigidbody.rotation * localVelocityOverride;
                    //    rigidbody.MoveRotation(Quaternion.AngleAxis((player.screenMouseRatio * player.mouseSensitivity * mouseInput.x) * Time.fixedDeltaTime, Vector3.up) * rigidbody.rotation);
                    //}

                    if (jumpRb) {
                        rigidbody.velocity += new Vector3(0f, 4f, 0f);
                        animator.SetTrigger("TRI_jump");
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

            private void TestFreeRoamEvent() {
                player.SetState(StateId.Player.MoveModes.Grounded.freeRoam);
                player.weaponController.aimingWeapon = false;
            }
        }
    }
}