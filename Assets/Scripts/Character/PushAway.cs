using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerControl;

[RequireComponent(typeof(CapsuleCollider))]
public class PushAway : MonoBehaviour {

    public float sqrVelocityThreshold = 0.009f;
    public float pushAwayScalar = 0.5f;

    private CapsuleCollider playerPushAwayCollider;
    private PlayerController player;

    private new Rigidbody rigidbody;
    private float timeStationary = 0.0f;
    private Vector3 actualLocalCenter;

    private Collider[] colliders;

    void Start() {
        playerPushAwayCollider = gameObject.GetComponent<CapsuleCollider>();
        player = gameObject.GetComponentInParent<PlayerController>();
        rigidbody = gameObject.GetComponentInParent<Rigidbody>();

        actualLocalCenter = playerPushAwayCollider.center + transform.localPosition;

        colliders = new Collider[32];
    }

    void Update() {
        if (rigidbody.velocity.sqrMagnitude < sqrVelocityThreshold) {
            PushCharAway();
        }
    }

    private void PushCharAway() {
        int numColliders = Physics.OverlapCapsuleNonAlloc(
            rigidbody.position + actualLocalCenter - (0.5f * playerPushAwayCollider.height) * Vector3.up,
            rigidbody.position + actualLocalCenter + (0.5f * playerPushAwayCollider.height) * Vector3.up,
            playerPushAwayCollider.radius,
            colliders,
            player.raycastMask);
        RaycastHit rcHit;
        for (int n = 0; n < numColliders; ++n) {
            if (Physics.Raycast(rigidbody.position + actualLocalCenter, colliders[n].transform.position, out rcHit, 100.0f, player.raycastMask)) {
                Debug.DrawLine(rigidbody.position + actualLocalCenter, colliders[n].transform.position, Color.cyan);
                rigidbody.AddForce(pushAwayScalar * rcHit.distance * -rcHit.normal);
            }
        }
    }
}
