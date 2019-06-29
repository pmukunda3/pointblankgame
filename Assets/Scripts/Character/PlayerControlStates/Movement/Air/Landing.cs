using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerControl {
    namespace State {
        public class Landing : PlayerControlState {

            public float groundCheckDistance = 0.18f;
            public float maxTurnSpeed = 1.0f;

            public float moveSpeedMultiplier = 1.0f;

            public AnimationCurve rollAnimationSpeedCurve;

            private Vector2 mouseInput;
            private Vector2 moveInput;

            private Rigidbody rigidbody;

            private int landMode = 0;
            private float rollAnimationSpeed = 1.0f;

            public new void Start() {
                base.Start();
                player.RegisterState(StateId.Player.MoveModes.Air.land, this);

                rigidbody = player.GetComponent<Rigidbody>();

                EventManager.StartListening<MecanimBehaviour.LandEvent>(new UnityEngine.Events.UnityAction(OnLandingEvent));
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
                else                       animator.SetBool("sprint", false);
            }

            public override void AnimatorMove(Vector3 localAnimatorVelocity, Vector3 localRigidbodyVelocity) {
                // Velocity is preserved in the rigidbody.velocity, but animator.velocity is (0, 0, 0) for the first time this is called.
                Vector3 playerVelocity;
                AnimatorStateInfo animState = animator.GetCurrentAnimatorStateInfo(0);
                switch (landMode) {
                    case 0:
                        //playerVelocity == rigidbody.velocity;
                        break;
                    case 1:
                        playerVelocity = animator.velocity;
                        playerVelocity.y = rigidbody.velocity.y;
                        if (animState.IsName("Roll Landing") || animState.IsName("Roll Landing Settle")) {
                            animator.speed = rollAnimationSpeed;
                        }
                        rigidbody.velocity = playerVelocity;
                        break;
                    case 2:
                        playerVelocity = animator.velocity;
                        playerVelocity.y = rigidbody.velocity.y;
                        rigidbody.velocity = playerVelocity;
                        break;
                }
            }

            public override void MoveRigidbody(Vector3 localRigidbodyVelocity) {
                // do nothing
            }

            public override void UpdateAnimator(Vector3 localRigidbodyVelocity) {
                animator.SetFloat("velLocalX", moveInput.x);
                animator.SetFloat("velLocalZ", moveInput.y);
            }

            public void OnLandingEvent() {
                player.shared.lastRigidbodyVelocity = rigidbody.velocity;
                landMode = animator.GetInteger("landMode");
                if (landMode == 1) {
                    Debug.Log((Quaternion.Inverse(rigidbody.rotation) * rigidbody.velocity).z);
                    rollAnimationSpeed = rollAnimationSpeedCurve.Evaluate((Quaternion.Inverse(rigidbody.rotation) * rigidbody.velocity).z);
                }

                player.SetState(StateId.Player.MoveModes.Air.land);
                player.weaponController.aimingWeapon = false;
            }
        }
    }
}