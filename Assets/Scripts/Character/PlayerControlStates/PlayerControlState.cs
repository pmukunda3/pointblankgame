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

        public abstract void UseInput(Vector2 moveInput, Vector2 mouseInput, UserInput.Actions actions);
        public abstract void AnimatorMove(Vector3 localAnimatorVelocity, Vector3 localRigidbodyVelocity);
        public abstract void UpdateAnimator(Vector3 localRigidbodyVelocity);
        public abstract void MoveRigidbody(Vector3 localRigidbodyVelocity);
        public abstract void CollisionEnter(Collision collision);

        //protected bool CheckClimbable() {
        //    RaycastHit hitInfo;

        //    Debug.DrawLine(rigidbody.position + (Vector3.up * 0.1f), rigidbody.position + (Vector3.up * 0.1f) + (Vector3.down * groundCheckDistance), Color.yellow);

        //    if (Physics.Raycast(rigidbody.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, groundCheckDistance, player.raycastMask)) {
        //        groundNormal = hitInfo.normal;
        //        groundPoint = hitInfo.point;
        //        return true;
        //    }
        //    else {
        //        groundNormal = Vector3.zero;
        //        return false;
        //    }
        //}

        //protected void StickToGroundHelper(float downwardDistance) {
        //    RaycastHit hitInfo;

        //    if (Physics.SphereCast(rigidbody.position, 0.15f, Vector3.down, out hitInfo, downwardDistance, player.raycastMask)) {
        //        if (Mathf.Abs(Vector3.Angle(hitInfo.normal, Vector3.up)) < 85f) {
        //            if (rigidbody.velocity.y < 0f) {
        //                rigidbody.velocity = Vector3.ProjectOnPlane(rigidbody.velocity, hitInfo.normal) + Vector3.up * rigidbody.velocity.y;
        //            }
        //            else {
        //                rigidbody.velocity = Vector3.ProjectOnPlane(rigidbody.velocity, hitInfo.normal);
        //            }
        //        }
        //    }
        //}
    }
}