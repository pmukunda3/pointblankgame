using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerControl {
    public abstract class PlayerControlState : MonoBehaviour {

        protected PlayerController player;
        protected Animator animator;

        public void Start() {
            player = gameObject.GetComponentInParent<PlayerController>();
            animator = gameObject.GetComponentInParent<Animator>();
        }

        public abstract void UseInput(Vector2 moveInput, Vector2 mouseInput, bool walk, bool sprint, bool crouch, bool jump, bool use, bool primaryFire, bool secondaryFire);
        public abstract void AnimatorMove(Vector3 localAnimatorVelocity, Vector3 localRigidbodyVelocity);
        public abstract void UpdateAnimator(Vector3 localRigidbodyVelocity);
        public abstract void MoveRigidbody(Vector3 localRigidbodyVelocity);
    }
}