using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    public bool ApplyRootMotion;
    public GameObject Weapon;
    public GameObject Camera;

    private Animator animator;

    private Transform LeftHandIKTarget, RightHandIKTarget;
    private Vector3 LookTarget;

    void Start()
    {
        animator = GetComponent<Animator>();    
    }

    void OnAnimatorMove()
    {
        if (ApplyRootMotion)
        {
            transform.position += animator.deltaPosition;
        }
        transform.parent.position = transform.position;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(Vector3.zero);
    }

    void OnAnimatorIK()
    {
        LookTarget = Camera.GetComponent<CameraController>().target;
        animator.SetLookAtWeight(1f);
        animator.SetLookAtPosition(LookTarget);

        LeftHandIKTarget = Weapon.GetComponent<WeaponManager>().activeWeapon.GetComponent<WeaponProperties>().LeftHandIKTarget;
        animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1f);
        animator.SetIKPosition(AvatarIKGoal.LeftHand, LeftHandIKTarget.position);
        animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1f);
        animator.SetIKRotation(AvatarIKGoal.LeftHand, LeftHandIKTarget.rotation);

        RightHandIKTarget = Weapon.GetComponent<WeaponManager>().activeWeapon.GetComponent<WeaponProperties>().RightHandIKTarget;        
        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1f);
        animator.SetIKPosition(AvatarIKGoal.RightHand, RightHandIKTarget.position);
        animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1f);
        animator.SetIKRotation(AvatarIKGoal.RightHand, RightHandIKTarget.rotation);
        
    }

}
