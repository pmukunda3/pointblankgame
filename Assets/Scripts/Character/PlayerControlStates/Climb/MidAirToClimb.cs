using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerControl {
    namespace State {
        public class MidAirToClimb : PlayerControlState {

            public ClimbAnimationHelper climbAnimHelper;
            public ClimbValidator climbValidator;

            private Vector2 moveInput;
            private Vector3 climbPoint;
            private new ClimbValidator.ClimbAnimation animation;

            private float elapsedTime = 0f;

            public new void Start() {
                base.Start();
                player.RegisterState(StateId.Player.MoveModes.Climb.midAirToClimb, this);

                EventManager.StartListening<MecanimBehaviour.MidAirToClimbEvent>(new UnityEngine.Events.UnityAction(OnMidAirClimbEvent));
            }

            public override void UseInput(Vector2 moveInput, Vector2 mouseInput, UserInput.Actions actions) {
                this.moveInput = moveInput;

                if (actions.sprint.down) animator.SetBool("sprint", true);
                if (actions.secondaryFire.down) animator.SetBool("aimMode", false);
                if (actions.crouch.down) animator.SetBool("crouch", true);
            }

            private void AnimationInit() {
                elapsedTime = 0f;
                if (climbValidator.GetClimbInfo(out climbPoint, out animation)) {
                    Debug.Log("Climb Point : " + climbPoint.ToString("F4") + ", anim # = " + animation);
                    animator.SetInteger("climbAnim", (int)animation - 1);
                }
            }

            public override void AnimatorMove(Vector3 localAnimatorVelocity, Vector3 localRigidbodyVelocity) {
                rigidbody.velocity = animator.velocity;
            }

            public override void AnimatorIK() {
                // do nothing
            }

            public override void MoveRigidbody(Vector3 localRigidbodyVelocity) {
                // do nothing, all movement will be done by the animation
            }

            public override void UpdateAnimator(Vector3 localRigidbodyVelocity) {
                if (elapsedTime > 0.05f) {
                    if (climbPoint == Vector3.zero) {
                        Debug.Log("climbPoint reset, for some reason");
                        Debug.Break();
                    }
                    climbAnimHelper.SetMatchTarget(climbPoint, animation, 0.15f);
                }
                elapsedTime += Time.fixedDeltaTime;
            }

            public override void CollisionEnter(Collision collision) {
                Debug.Break();
            }

            private void OnMidAirClimbEvent() {
                animator.speed = 1.0f;
                this.moveInput = player.GetLatestMoveInput();
                AnimationInit();

                player.SetState(StateId.Player.MoveModes.Climb.midAirToClimb);
            }
        }
    }
}