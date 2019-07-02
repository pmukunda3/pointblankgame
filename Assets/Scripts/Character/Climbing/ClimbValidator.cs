using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerControl;

public class ClimbValidator : MonoBehaviour
{
    [System.Serializable]
    public struct ClimbCapsules {
        public float lowClimbRadius;
        public float lowClimbLength;
        public float midClimbRadius;
        public float midClimbLength;
        public float highClimbRadius;
        public float highClimbLength;

        public Vector3 lowClimbCheckOffset;
        public Vector3 midClimbCheckOffset;
        public Vector3 highClimbCheckOffset;
    }

    public ClimbCapsules climbSettings;

    private new Rigidbody rigidbody;
    private PlayerController player;
    private Collider[] overlapBuffer = new Collider[32];

    public void Start() {
        rigidbody = gameObject.GetComponentInParent<Rigidbody>();
        player = gameObject.GetComponentInParent<PlayerController>();
    }

    protected bool CheckClimbable(float radius, float length, Vector3 offset) {
        RaycastHit hitInfo;

        //if (Physics.Raycast(rigidbody.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, groundCheckDistance, player.raycastMask)) {
        //    groundNormal = hitInfo.normal;
        //    groundPoint = hitInfo.point;
        //    return true;
        //}
        //else {
        //    groundNormal = Vector3.zero;
        //    return false;
        //}

        int collidersSize = Physics.OverlapCapsuleNonAlloc(rigidbody.position + rigidbody.rotation * (offset + (length * 0.5f * Vector3.left)),
            rigidbody.position + rigidbody.rotation * (offset + (length * 0.5f * Vector3.right)), radius, overlapBuffer, player.raycastMask);

        if (collidersSize == 0) return false;
        else return false;
    }

    protected void StickToGroundHelper(float downwardDistance) {
        RaycastHit hitInfo;

        if (Physics.SphereCast(rigidbody.position, 0.15f, Vector3.down, out hitInfo, downwardDistance, player.raycastMask)) {
            if (Mathf.Abs(Vector3.Angle(hitInfo.normal, Vector3.up)) < 85f) {
                if (rigidbody.velocity.y < 0f) {
                    rigidbody.velocity = Vector3.ProjectOnPlane(rigidbody.velocity, hitInfo.normal) + Vector3.up * rigidbody.velocity.y;
                }
                else {
                    rigidbody.velocity = Vector3.ProjectOnPlane(rigidbody.velocity, hitInfo.normal);
                }
            }
        }
    }

    private bool drawGizmo = false;
    public void OnDrawGizmos() {

    }
}
