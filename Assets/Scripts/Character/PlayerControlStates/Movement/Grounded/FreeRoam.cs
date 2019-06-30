﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerControl {
    namespace State {
        public class FreeRoam : Grounded {

            private IMovementState freeRoamMovement;

            private Vector3 direction = Vector3.zero;

            public new void Start() {
                base.Start();
                player.RegisterState(StateId.Player.MoveModes.Grounded.freeRoam, this);

                freeRoamMovement = gameObject.GetComponentInChildren<Running>() as IMovementState;

                EventManager.StartListening<MecanimBehaviour.FreeRoamEvent>(new UnityEngine.Events.UnityAction(OnFreeRoamEvent));
            }

            public override void UseInput(Vector2 moveInput, Vector2 mouseInput, UserInput.Actions actions) {
                base.UseInput(moveInput, mouseInput, actions);

                if (actions.walk.active) {
                    if (moveInput.sqrMagnitude > 0.3f * 0.3f) {
                        moveInput = moveInput.normalized * 0.3f;
                    }
                }

                    // TODO: Fix this.
                float extraRotation = Mathf.Clamp(mouseInput.x, -maxTurnSpeed, maxTurnSpeed);
                rigidbody.velocity = Quaternion.AngleAxis(player.screenMouseRatio * player.mouseSensitivity * extraRotation * Time.deltaTime, Vector3.up) * rigidbody.velocity;

                if (actions.sprint.active) animator.SetBool("sprint", true);
                if (actions.secondaryFire.down) animator.SetBool("aimMode", true);
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
                    //rigidbody.MoveRotation(Quaternion.AngleAxis(player.screenMouseRatio * player.mouseSensitivity * mouseInput.x * Time.fixedDeltaTime, Vector3.up) * rigidbody.rotation);
                    rigidbody.rotation = Quaternion.Euler(0f, player.AimYaw(), 0f);
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

            private void OnFreeRoamEvent() {
                jumpInput = false;
                player.weaponController.aimingWeapon = false;
                animator.speed = 1.0f;

                this.moveInput = player.GetLatestMoveInput();

                player.SetState(StateId.Player.MoveModes.Grounded.freeRoam);
            }
        }
    }
}