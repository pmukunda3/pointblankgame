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

            private bool kinematicControl = false;
            private float elapsedTime = 0f;

            public new void Start() {
                base.Start();
                player.RegisterState(StateId.Player.MoveModes.Climb.midAirToClimb, this);

                EventManager.StartListening<MecanimBehaviour.MidAirToClimbEvent>(new UnityEngine.Events.UnityAction(OnMidAirClimbEvent));
                EventManager.StartListening<MecanimBehaviour.MidAirToClimbEventExit>(new UnityEngine.Events.UnityAction(OnMidAirClimbEventExit));
            }

            public override void UseInput(Vector2 moveInput, Vector2 mouseInput, UserInput.Actions actions) {
                this.moveInput = moveInput;

                if (actions.sprint.down) animator.SetBool("sprint", true);
                if (actions.aim.down) animator.SetBool("aimMode", false);
                if (actions.crouch.down) animator.SetBool("crouch", true);
            }

            private void AnimationInit() {
                elapsedTime = 0f;
                if (climbValidator.ClimbValid()) {
                    this.climbPoint = climbValidator.GetClimbPoint();
                    this.animation = climbValidator.GetClimbAnimation();
                    Debug.Log("Climb Point : " + climbPoint.ToString("F4") + ", anim # = " + animation);
                    climbValidator.Invalidate();
                }
            }

            public override void AnimatorMove(Vector3 localAnimatorVelocity, Vector3 localRigidbodyVelocity) {
                if (kinematicControl) {
                    rigidbody.MovePosition(rigidbody.position + animator.deltaPosition);
                }
                else {
                    rigidbody.velocity = animator.velocity;
                }
            }

            public override void AnimatorIK() {
                // do nothing
            }

            public override void MoveRigidbody(Vector3 localRigidbodyVelocity) {
                if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > climbAnimHelper.GetAnimEnd(animation)) {
                    if (kinematicControl) {
                        kinematicControl = false;
                        rigidbody.isKinematic = kinematicControl;
                    }
                }
                else {
                    if (!kinematicControl) {
                        kinematicControl = true;
                        rigidbody.isKinematic = kinematicControl;
                    }
                }
                elapsedTime += Time.fixedDeltaTime;
            }

            public override void UpdateAnimator(Vector3 localRigidbodyVelocity) {
                if (elapsedTime > 0.05f) {
                    climbAnimHelper.SetMatchTarget(climbPoint, animation, 0.15f);
                }
                elapsedTime += Time.fixedDeltaTime;
            }

            public override void CollisionEnter(Collision collision) {
                //Debug.Break();
            }

            private void OnMidAirClimbEvent() {
                animator.speed = 1.0f;
                this.moveInput = player.GetLatestMoveInput();
                AnimationInit();
                rigidbody.isKinematic = true;
                kinematicControl = true;
                //rigidbody.useGravity = false;
                player.legsCollider.enabled = false;

                player.SetState(StateId.Player.MoveModes.Climb.midAirToClimb);
            }

            private void OnMidAirClimbEventExit() {
                rigidbody.isKinematic = false;
                kinematicControl = false;
                //rigidbody.useGravity = true;
                player.legsCollider.enabled = true;
            }
        }
    }
}