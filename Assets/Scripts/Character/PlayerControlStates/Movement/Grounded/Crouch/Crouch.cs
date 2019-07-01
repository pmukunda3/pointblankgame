using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerControl {
    namespace State {
        public class Crouch : Grounded {

            public AnimationCurve realignSpeed;
            public float timeToCompleteTurn;

            private IMovementState crouchDecel;

            private bool realignCharModel = true;

            private Quaternion startRot;
            private Quaternion endRot;
            private float angleDiff;
            private float turnSpeed;
            private float elapsedTime;
            private float turnInterp;

            public new void Start() {
                base.Start();
                player.RegisterState(StateId.Player.MoveModes.Grounded.Crouch.freeLook, this);

                crouchDecel = gameObject.GetComponentInChildren<Crouching>() as IMovementState;

                EventManager.StartListening<MecanimBehaviour.CrouchEvent>(new UnityEngine.Events.UnityAction(OnCrouchEvent));
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
                if (actions.secondaryFire.down) animator.SetBool("aimMode", true);
                if (actions.crouch.down) animator.SetBool("crouch", false);
                if (actions.jump.down) animator.SetBool("crouch", false);

                // TODO: movement input will cause character to get up
            }

            public override void AnimatorMove(Vector3 localAnimatorVelocity, Vector3 localRigidbodyVelocity) {
                // do nothing
            }

            public override void MoveRigidbody(Vector3 localRigidbodyVelocity) {
                if (CheckGrounded()) {
                    if (rigidbody.velocity.sqrMagnitude > 0.0001f) {
                        MovementChange moveChange = crouchDecel.CalculateAcceleration(moveInput, localRigidbodyVelocity, Time.fixedDeltaTime);
                        rigidbody.AddRelativeForce(moveChange.localAcceleration, ForceMode.Acceleration);
                    }

                    if (!realignCharModel && Mathf.Abs(player.LookToMoveAngle()) > 60f) {
                        InitRealignCharModel();
                    }
                    else if (Mathf.Abs(player.LookToMoveAngle()) < 12f) {
                        realignCharModel = false;
                    }

                    if (realignCharModel) {
                        float adjustedTurnSpeed = turnSpeed * realignSpeed.Evaluate(elapsedTime / timeToCompleteTurn);
                        turnInterp += adjustedTurnSpeed / angleDiff * Time.fixedDeltaTime;
                        elapsedTime += Time.fixedDeltaTime;

                        rigidbody.MoveRotation(Quaternion.Slerp(startRot, endRot, turnInterp));
                        if (turnInterp >= 1.0f) realignCharModel = false;
                    }
                }
                else {
                    animator.SetBool("crouch", false);
                    animator.SetBool("grounded", false);
                    animator.SetTrigger("TRG_fall");
                }
            }

            public override void UpdateAnimator(Vector3 localRigidbodyVelocity) {
                animator.SetFloat("velLocalX", 0.0f);
                animator.SetFloat("velLocalZ", 0.0f);
            }

            public override void CollisionEnter(Collision collision) {
                // do nothing
            }

            private void InitRealignCharModel() {
                realignCharModel = true;
                startRot = rigidbody.rotation;
                endRot = player.AimYawQuaternion();
                angleDiff = Quaternion.Angle(rigidbody.rotation, player.AimYawQuaternion());
                turnSpeed = angleDiff / timeToCompleteTurn;
                elapsedTime = 0f;
                turnInterp = 0f;
            }

            private void OnCrouchEvent() {
                InitRealignCharModel();
                player.weaponController.aimingWeapon = false;
                animator.speed = 1.0f;

                this.moveInput = player.GetLatestMoveInput();

                player.SetState(StateId.Player.MoveModes.Grounded.Crouch.freeLook);
            }
        }
    }
}