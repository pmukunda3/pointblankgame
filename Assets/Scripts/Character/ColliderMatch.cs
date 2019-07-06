using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
public class ColliderMatch : MonoBehaviour {
    public Transform leftKnee;
    public Transform rightKnee;

    private CapsuleCollider playerCollider;

    private Vector3 origPos;
    private Vector3 origPosLeftKnee;
    private Vector3 origPosRightKnee;

    public float heightThreshold;

    void Start() {
        playerCollider = gameObject.GetComponent<CapsuleCollider>();

        origPos = transform.localPosition;
        origPosLeftKnee = leftKnee.position;
        origPosRightKnee = rightKnee.position;
    }

    // TODO: Have the knee affect the characters collider
    void Update() {
        //if (leftKnee.position.y - origPosLeftKnee.position.y)
    }
}
