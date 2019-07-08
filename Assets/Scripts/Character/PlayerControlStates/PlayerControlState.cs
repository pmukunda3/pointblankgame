using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerControl {
    public abstract class PlayerControlState : MonoBehaviour {

        protected PlayerController player;
        protected Animator animator;

        protected new Rigidbody rigidbody;

        public void Start() {
            player = gameObject.GetComponentInParent<PlayerController>();
            animator = gameObject.GetComponentInParent<Animator>();
            rigidbody = gameObject.GetComponentInParent<Rigidbody>();
        }

        public abstract void UseInput(Vector2 moveInput, Vector2 mouseInput, UserInput.Actions actions);
        public abstract void AnimatorMove(Vector3 localAnimatorVelocity, Vector3 localRigidbodyVelocity);
        public abstract void AnimatorIK();
        public abstract void UpdateAnimator(Vector3 localRigidbodyVelocity);
        public abstract void MoveRigidbody(Vector3 localRigidbodyVelocity);
        public abstract void CollisionEnter(Collision collision);
    }
}