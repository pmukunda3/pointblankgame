﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerControl {
    namespace State {
        public class Falling : PlayerControlState {

            public float groundCheckDistanceMinimum = 0.11f;
            public float landAnimationFrameTarget = 3f;
            public float landAnimationFrameRate = 30f;
            public float maxTurnSpeed = 0.2f;

            private Vector2 mouseInput;
            private Vector2 moveInput;

            private IMovementState airControlMovement;

            private Rigidbody rigidbody;
            private Vector3 groundNormal = Vector3.zero;
            private Vector3 groundPoint = Vector3.zero;

            public new void Start() {
                base.Start();
                player.RegisterState(PlayerStateId.MoveModes.Air.falling, this);

                rigidbody = player.GetComponent<Rigidbody>();

                airControlMovement = gameObject.GetComponentInChildren<AirControlFromFall>() as IMovementState;
            }

            public override void AnimatorMove(Vector3 localAnimatorVelocity, Vector3 localRigidbodyVelocity) {
                // do nothing
            }

            public override void MoveRigidbody(Vector3 localRigidbodyVelocity) {
                MovementChange moveChange = airControlMovement.CalculateAcceleration(moveInput, localRigidbodyVelocity, Time.fixedDeltaTime);
                rigidbody.AddRelativeForce(moveChange.localAcceleration, ForceMode.Acceleration);

                CheckLandingDistance();
            }

            public override void UpdateAnimator(Vector3 localRigidbodyVelocity) {
                animator.SetFloat("velLocalX", 0f);
                animator.SetFloat("velLocalZ", 0f);
                animator.SetFloat("velLocalY", localRigidbodyVelocity.y);
            }

            public override void UseInput(Vector2 moveInput, Vector2 mouseInput, UserInput.Actions actions) {
                //if (!sprint) Debug.Log("Sprint Released");

                this.moveInput = moveInput;
                this.mouseInput = mouseInput;

                float extraRotation = Mathf.Clamp(mouseInput.x, -maxTurnSpeed, maxTurnSpeed);
                rigidbody.velocity = Quaternion.AngleAxis(player.screenMouseRatio * player.mouseSensitivity * extraRotation * Time.deltaTime, Vector3.up) * rigidbody.velocity;
            }

            private void CheckLandingDistance() {
                RaycastHit hitInfo;

                float checkDistance = Mathf.Max(landAnimationFrameTarget / landAnimationFrameRate * -rigidbody.velocity.y, groundCheckDistanceMinimum);
                Debug.DrawLine(player.transform.position + (Vector3.up * 0.1f), player.transform.position + (Vector3.up * 0.1f) + (Vector3.down * (checkDistance + 0.1f)), Color.yellow);

                if (checkDistance > 0.0f && Physics.Raycast(player.transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, (checkDistance + 0.1f), player.raycastMask)) {
                    if (rigidbody.velocity.y < -8f) {
                        player.landingAnimation = 0;
                    }
                    else {
                        player.landingAnimation = 1;
                    }
                    animator.SetBool("grounded", true);
                }
            }
        }
    }
}