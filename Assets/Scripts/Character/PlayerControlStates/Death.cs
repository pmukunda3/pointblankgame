using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerControl {
    namespace State {
        public class Death : PlayerControlState {

            private Vector2 mouseInput;

            public new void Start() {
                base.Start();
                player.RegisterState(StateId.Player.death, this);

                EventManager.StartListening<PlayerDeathEvent>(new UnityEngine.Events.UnityAction(OnDeathEvent));
            }

            public override void UseInput(Vector2 moveInput, Vector2 mouseInput, UserInput.Actions actions) {
                this.mouseInput = mouseInput;
            }

            public override void AnimatorMove(Vector3 localAnimatorVelocity, Vector3 localRigidbodyVelocity) {
                Vector3 playerVelocity = animator.velocity;

                playerVelocity.y = rigidbody.velocity.y;
                rigidbody.velocity = playerVelocity;
            }

            public override void AnimatorIK() {
                // do nothing
            }

            public override void MoveRigidbody(Vector3 localRigidbodyVelocity) {
                rigidbody.velocity = Vector3.zero;
            }

            public override void UpdateAnimator(Vector3 localRigidbodyVelocity) {
                // do nothing
            }

            private void OnDeathEvent() {
                animator.SetTrigger("TRG_death");
                animator.SetFloat("velLocalX", 0.0f);
                animator.SetFloat("velLocalZ", 0.0f);

                player.SetState(StateId.Player.death);
            }

            public override void CollisionEnter(Collision collision) {
                // do nothing
            }
        }
    }
}