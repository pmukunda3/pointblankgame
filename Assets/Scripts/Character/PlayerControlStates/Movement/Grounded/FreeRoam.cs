﻿using System.Collections;
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

            private Rigidbody rigidbody;
            private Vector3 groundNormal = Vector3.zero;
            private Vector3 groundPoint = Vector3.zero;

            private float lateralSpeed = 0f;
            private Vector3 direction = Vector3.zero;

            public new void Start() {
                base.Start();
                player.RegisterState(PlayerStateId.MoveModes.Grounded.freeRoam, this);

                rigidbody = player.GetComponent<Rigidbody>();
                freeRoamMovement = gameObject.GetComponentInChildren<Running>() as IMovementState;
            }

            public override void UseInput(Vector2 moveInput, Vector2 mouseInput, bool walk, bool sprint, bool crouch, bool jump, bool use, bool primaryFire, bool secondaryFire) {
                //if (sprint) Debug.Log("Sprint Pressed");

                if (walk) {
                    if (moveInput.sqrMagnitude > 0.3f * 0.3f) {
                        moveInput = moveInput.normalized * 0.3f;
                    }
                }

                this.moveInput = moveInput;
                this.mouseInput = mouseInput;

                //float extraRotation = Mathf.Clamp(mouseInput.x, -maxTurnSpeed, maxTurnSpeed);
                //rigidbody.velocity = Quaternion.AngleAxis(player.screenMouseRatio * player.mouseSensitivity * extraRotation * Time.deltaTime, Vector3.up) * rigidbody.velocity;

                if (sprint) animator.SetBool("sprint", true);
                if (secondaryFire) animator.SetBool("aimMode", true);
            }

            public override void AnimatorMove(Vector3 localAnimatorVelocity, Vector3 localRigidbodyVelocity) {
                MovementChange moveChange = freeRoamMovement.CalculateAcceleration(moveInput, localRigidbodyVelocity, Time.deltaTime);

                Vector3 newVelocity = animator.velocity * moveSpeedMultiplier;
                newVelocity.y = rigidbody.velocity.y;

                lateralSpeed += moveChange.localAcceleration.x * Time.deltaTime;

                //rigidbody.velocity = newVelocity;
                //rigidbody.AddForce(rigidbody.rotation * moveChange.localAcceleration, ForceMode.Acceleration);
                rigidbody.velocity = newVelocity + rigidbody.rotation * new Vector3(lateralSpeed, 0f, 0f);

                //Debug.DrawRay(rigidbody.position + 0.7f * Vector3.up, localRigidbodyVelocity, Color.blue);
                //Debug.DrawRay(rigidbody.position + 0.8f * Vector3.up, rigidbody.rotation * moveChange.localAcceleration, Color.cyan);
                //Debug.DrawRay(rigidbody.position + 0.9f * Vector3.up, newVelocity, Color.magenta);

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
                }
                else {
                    //animator.SetBool("grounded", false);
                    Debug.Log("SetBool('grounded', false);");
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