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

                rigidbody = player.GetComponent<Rigidbody>();

                EventManager.StartListening<MecanimBehaviour.AimingEvent>(new UnityEngine.Events.UnityAction(TestAimingEvent));
            }

            public override void UseInput(Vector2 moveInput, Vector2 mouseInput, UserInput.Actions actions) {
                base.UseInput(moveInput, mouseInput, actions);

                if (actions.walk.active) {
                    if (moveInput.sqrMagnitude > 0.3f * 0.3f) {
                        moveInput = moveInput.normalized * 0.3f;
                    }
                }

                float extraRotation = Mathf.Clamp(mouseInput.x, -maxTurnSpeed, maxTurnSpeed);
                rigidbody.velocity = Quaternion.AngleAxis(player.screenMouseRatio * player.mouseSensitivity * extraRotation * Time.deltaTime, Vector3.up) * rigidbody.velocity;

                if (actions.sprint.down) animator.SetBool("sprint", true);
                if (actions.secondaryFire.down) animator.SetBool("aimMode", false);
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