using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerControl {
    namespace State {
        public class Crouch : Grounded {

            private Quaternion currentAim;

            public new void Start() {
                base.Start();
                player.RegisterState(StateId.Player.MoveModes.Grounded.Crouch.freeLook, this);

                EventManager.StartListening<MecanimBehaviour.CrouchEvent>(new UnityEngine.Events.UnityAction(OnCrouchEvent));
            }

            public override void UseInput(Vector2 moveInput, Vector2 mouseInput, UserInput.Actions actions) {
                base.UseInput(moveInput, mouseInput, actions);

                if (actions.walk.active) {
                    if (moveInput.sqrMagnitude > 0.3f * 0.3f) {
                        moveInput = moveInput.normalized * 0.3f;
                    }
                }

                if (actions.sprint.active) animator.SetBool("sprint", true);
                if (actions.secondaryFire.down) animator.SetBool("aimMode", true);
            }

            public override void AnimatorMove(Vector3 localAnimatorVelocity, Vector3 localRigidbodyVelocity) {
                // do nothing
            }

            public override void MoveRigidbody(Vector3 localRigidbodyVelocity) {
                if (CheckGrounded()) {
                    rigidbody.MoveRotation(Quaternion.AngleAxis(player.screenMouseRatio * player.mouseSensitivity * mouseInput.x * Time.fixedDeltaTime, Vector3.up) * rigidbody.rotation);
                }
                else {
                    animator.SetBool("grounded", false);
                    animator.SetTrigger("TRG_fall");
                }
            }

            public override void UpdateAnimator(Vector3 localRigidbodyVelocity) {
                animator.SetFloat("velLocalX", 0.0f);
                animator.SetFloat("velLocalZ", 0.0f);
            }

            private void OnCrouchEvent() {
                jumpInput = false;
                player.weaponController.aimingWeapon = false;
                animator.speed = 1.0f;

                this.moveInput = player.GetLatestMoveInput();

                player.SetState(StateId.Player.MoveModes.Grounded.Crouch.freeLook);
            }
        }
    }
}