using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
public class ColliderMatchFeet : MonoBehaviour {
    public Transform leftFoot;
    public Transform rightFoot;

    //public bool matchCollider = false;

    private CapsuleCollider playerCollider;
    private Rigidbody rigidbody;

    private Vector3 origLocalPos;
    private Vector3 origPosLeftFoot;
    private Vector3 origLocalPosLeftFoot;
    private Vector3 origPosRightFoot;
    private Vector3 origLocalPosRightFoot;

    public float heightThreshold;

    void Start() {
        playerCollider = gameObject.GetComponent<CapsuleCollider>();

        origLocalPos = playerCollider.center; // transform.localPosition;
        origPosLeftFoot = leftFoot.position;
        origPosRightFoot = rightFoot.position;

        //matchCollider = false;
        rigidbody = gameObject.GetComponentInParent<Rigidbody>();
    }

    void FixedUpdate() {
        float leftFootHeightDiff  = leftFoot.position.y - rigidbody.position.y - origPosLeftFoot.y;
        float rightFootHeightDiff = rightFoot.position.y - rigidbody.position.y - origPosRightFoot.y;

        if (leftFootHeightDiff > heightThreshold && rightFootHeightDiff > heightThreshold) {
            //Debug.Log("MOVED LEGS: L=" + leftFootHeightDiff + ", R=" + rightFootHeightDiff);
            if (leftFootHeightDiff > rightFootHeightDiff) {
                //playerCollider.transform.localPosition = origLocalPos + rightFootHeightDiff * Vector3.up;
                playerCollider.center = origLocalPos + rightFootHeightDiff * Vector3.up;
            }
            else {
                //playerCollider.transform.localPosition = origLocalPos + leftFootHeightDiff * Vector3.up;
                playerCollider.center = origLocalPos + leftFootHeightDiff * Vector3.up;
            }
        }
        else {
            playerCollider.center = origLocalPos;
        }
    }
}
