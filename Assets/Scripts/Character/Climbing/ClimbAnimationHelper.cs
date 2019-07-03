using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbAnimationHelper : MonoBehaviour
{
    public float stepAnimStart;
    public float stepAnimEnd;
    public float lowAnimStart;
    public float lowAnimEnd;
    public float midAnimStart;
    public float midAnimEnd;
    public float highAnimStart;
    public float highAnimEnd;

    private Animator animator;
    private new Rigidbody rigidbody;

    public void Start() {
        animator = gameObject.GetComponentInParent<Animator>();
        rigidbody = gameObject.GetComponentInParent<Rigidbody>();
    }

    public void SetMatchTarget(Vector3 climbPoint, ClimbValidator.ClimbAnimation animation, float colliderRadius) {
        switch (animation) {
            case ClimbValidator.ClimbAnimation.Step:
                animator.MatchTarget(climbPoint + colliderRadius * Vector3.up, rigidbody.rotation, AvatarTarget.RightFoot, new MatchTargetWeightMask(0.5f * Vector3.one, 1f), stepAnimStart, stepAnimEnd);
                break;
            case ClimbValidator.ClimbAnimation.Low:
                animator.MatchTarget(climbPoint + colliderRadius * Vector3.up, rigidbody.rotation, AvatarTarget.RightHand, new MatchTargetWeightMask(1.0f * new Vector3(0.0f, 1.0f, 1.0f), 1f), lowAnimStart, lowAnimEnd);
                break;
            case ClimbValidator.ClimbAnimation.Mid:
                animator.MatchTarget(climbPoint + colliderRadius * Vector3.up, rigidbody.rotation, AvatarTarget.RightHand, new MatchTargetWeightMask(1.0f * Vector3.one, 1f), midAnimStart, midAnimEnd);
                break;
            case ClimbValidator.ClimbAnimation.High:
                animator.MatchTarget(climbPoint + colliderRadius * Vector3.up, rigidbody.rotation, AvatarTarget.RightHand, new MatchTargetWeightMask(1.0f * Vector3.one, 1f), highAnimStart, highAnimEnd);
                break;
        }
    }
}
