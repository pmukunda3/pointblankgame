using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerControl {
    namespace State {
        public class CrouchAiming : Grounded {

            public new void Start() {
                base.Start();
                player.RegisterState(StateId.Player.MoveModes.Grounded.Crouch.aiming, this);

                EventManager.StartListening<MecanimBehaviour.CrouchAimingEvent>(new UnityEngine.Events.UnityAction(OnCrouchAimingEvent));
            }

            public override void UseInput(Vector2 moveInput, Vector2 mouseInput, UserInput.Actions actions) {
                base.UseInput(moveInput, mouseInput, actions);

                if (actions.walk.active) {
                    if (moveInput.sqrMagnitude > 0.3f * 0.3f) {
                        moveInput = moveInput.normalized * 0.3f;
                    }
                }

                if (actions.sprint.active) {
                    animator.SetBool("sprint", true);
                    animator.SetBool("crouch", false);
                }

                if (actions.secondaryFire.down) animator.SetBool("aimMode", false);
                if (actions.crouch.down) animator.SetBool("crouch", false);
                if (actions.jump.down) animator.SetBool("crouch", false);
            }

            public override void AnimatorMove(Vector3 localAnimatorVelocity, Vector3 localRigidbodyVelocity) {
                // do nothing
            }

            public override void MoveRigidbody(Vector3 localRigidbodyVelocity) {
                if (CheckGrounded()) {
                    rigidbody.MoveRotation(Quaternion.Euler(0f, player.AimYaw(), 0f));
                }
                else {
                    animator.SetBool("crouch", false);
                    animator.SetBool("grounded", false);
                    animator.SetTrigger("TRG_fall");
                }
            }

            public override void UpdateAnimator(Vector3 localRigidbodyVelocity) {
                animator.SetFloat("velLocalX", moveInput.x);
                animator.SetFloat("velLocalZ", moveInput.y);
            }

            private void OnCrouchAimingEvent() {
                player.weaponController.aimingWeapon = true;
                animator.SetBool("sprint", false);
                animator.speed = 1.0f;

                this.moveInput = player.GetLatestMoveInput();

                player.SetState(StateId.Player.MoveModes.Grounded.Crouch.aiming);
            }
        }
    }
}