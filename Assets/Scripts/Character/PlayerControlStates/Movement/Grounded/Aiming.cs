using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerControl {
    namespace State {
        public class Aiming : Grounded {

            private Vector3 velocityReset = new Vector3(0f, 1f, 0f);

            public new void Start() {
                base.Start();
                player.RegisterState(StateId.Player.MoveModes.Grounded.aiming, this);

                EventManager.StartListening<MecanimBehaviour.AimingEvent>(new UnityEngine.Events.UnityAction(OnAimingEvent));
            }

            public override void UseInput(Vector2 moveInput, Vector2 mouseInput, UserInput.Actions actions) {
                base.UseInput(moveInput, mouseInput, actions);

                if (actions.walk.active) {
                    if (moveInput.sqrMagnitude > 0.3f * 0.3f) {
                        moveInput = moveInput.normalized * 0.3f;
                    }
                }

                if (actions.sprint.down) animator.SetBool("sprint", true);
                if (actions.secondaryFire.down) animator.SetBool("aimMode", false);
                if (actions.crouch.down) animator.SetBool("crouch", true);

                // TODO: movement input will cause character to get up
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
                    rigidbody.MoveRotation(Quaternion.Euler(0f, player.AimYaw(), 0f));
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

            private void OnAimingEvent() {
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