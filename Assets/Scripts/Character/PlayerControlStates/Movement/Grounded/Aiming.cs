using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerControl {
    namespace State {
        public class Aiming : Grounded {

            public CameraControl.State.Aiming cameraState;

            private Transform LeftHandIKTarget, RightHandIKTarget;
            private Vector3 LookTarget;

            public new void Start() {
                base.Start();
                player.RegisterState(StateId.Player.MoveModes.Grounded.aiming, this);

                EventManager.StartListening<MecanimBehaviour.AimingEvent>(new UnityEngine.Events.UnityAction(OnAimingEvent));
            }

            public override void UseInput(Vector2 moveInput, Vector2 mouseInput, UserInput.Actions actions) {
                base.UseInput(moveInput, mouseInput, actions);

                if (actions.walk.active) {
                    if (this.moveInput.sqrMagnitude > 0.3535f * 0.3535f) {
                        this.moveInput = this.moveInput.normalized * 0.3535f;
                    }
                }

                if (actions.primaryFire.down) weaponManager.FireWeaponDown();                
                if (actions.primaryFire.up) weaponManager.FireWeaponUp();
                if (actions.primaryFire.active) weaponManager.FireWeapon();

                if (actions.throwItem.down) animator.SetTrigger("Throw");

                if (actions.sprint.down) animator.SetBool("sprint", true);
                if (actions.aim.down) animator.SetBool("aimMode", false);
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

            public override void AnimatorIK() {
                LookTarget = cameraState.target;
                animator.SetLookAtWeight(1f);
                animator.SetLookAtPosition(LookTarget);

                LeftHandIKTarget = weaponManager.activeWeapon.GetComponent<WeaponIK>().LeftHandIKTarget;
                animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1f);
                animator.SetIKPosition(AvatarIKGoal.LeftHand, LeftHandIKTarget.position);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1f);
                animator.SetIKRotation(AvatarIKGoal.LeftHand, LeftHandIKTarget.rotation);


                // Thrower can override right hand IK
                if (animator.GetCurrentAnimatorStateInfo(animator.GetLayerIndex("Throwing")).IsName("Throwing"))
                {
                    thrower.AnimatorIK();
                }
                else
                { 
                    RightHandIKTarget = weaponManager.activeWeapon.GetComponent<WeaponIK>().RightHandIKTarget;
                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1f);
                    animator.SetIKPosition(AvatarIKGoal.RightHand, RightHandIKTarget.position);
                    animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1f);
                    animator.SetIKRotation(AvatarIKGoal.RightHand, RightHandIKTarget.rotation);
                }
            }

            public override void MoveRigidbody(Vector3 localRigidbodyVelocity) {
                base.MoveRigidbody(localRigidbodyVelocity);

                if (CheckGrounded()) {
                    rigidbody.MoveRotation(Quaternion.Euler(0f, player.AimYaw(), 0f));
                    StickToGroundHelper(0.35f);
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
                animator.SetBool("sprint", false);
                animator.speed = 1.0f;
                player.legsCollider.enabled = true;
                this.moveInput = player.GetLatestMoveInput();

                player.SetState(StateId.Player.MoveModes.Grounded.aiming);
            }
        }
    }
}