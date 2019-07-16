using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerControl;

public class ClimbAttemptEvent : UnityEngine.Events.UnityEvent { }

public class ClimbValidator : MonoBehaviour {
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

        public AnimationCurve maxClimbHeight;
    }

    private struct Range {
        public float low;
        public float high;
    }

    public struct ClimbInfo {
        public int candidatePointNum;
        public Vector3[] candidatePoints;
        public Vector3[] candidatePointsNormals;
    }

    public enum ClimbAnimation : int {
        None = 0,
        Step,
        Low,
        Mid,
        High,
    }

    public ClimbCapsules climbSettings;
    public int numberRaycasts;
    public Transform pivot;

    public float climbableDepth = 0.3f;
    public float climbableSlopeAngle = 30f;
    private float climbableSlopeRad;

    private Vector3 offsetMask = new Vector3(1.0f, 0.0f, 1.0f);

    private new Rigidbody rigidbody;
    private PlayerController player;
    private Collider[] overlapBuffer = new Collider[32];

    private new ClimbAnimation animation;
    private Vector3 climbPoint = Vector3.zero;
    private bool climbValid = false;

    public void Start() {
        rigidbody = gameObject.GetComponentInParent<Rigidbody>();
        player = gameObject.GetComponentInParent<PlayerController>();

        //EventManager.StartListening<ClimbAttemptEvent>(new UnityEngine.Events.UnityAction(OnClimbAttempt));

        climbableSlopeRad = climbableSlopeAngle * Mathf.PI;
        Debug.Log("Climbable Slope Rise Over Run = " + climbableSlopeRad);
    }

    public bool ClimbValid() {
        return climbValid;
    }

    public void Invalidate() {
        climbValid = false;
    }

    public Vector3 GetClimbPoint() {
        if (climbValid) {
            return climbPoint;
        }
        else {
            return Vector3.zero;
        }
    }

    public ClimbAnimation GetClimbAnimation() {
        if (climbValid) {
            return animation;
        }
        else {
            return ClimbAnimation.None;
        }
    }

    private bool CheckClimbVolume(out Range range) {
        range = new Range();
        bool lowClimb  = OverlapCapsuleInFront(climbSettings.lowClimbRadius, climbSettings.lowClimbLength, climbSettings.lowClimbCheckOffset);
        bool midClimb  = OverlapCapsuleInFront(climbSettings.midClimbRadius, climbSettings.midClimbLength, climbSettings.midClimbCheckOffset);
        bool highClimb = OverlapCapsuleInFront(climbSettings.highClimbRadius, climbSettings.highClimbLength, climbSettings.highClimbCheckOffset);

        if (lowClimb) {
            range.low = pivot.localPosition.y + climbSettings.lowClimbCheckOffset.y - climbSettings.lowClimbRadius + (player.AimDirection() * Vector3.Scale(offsetMask, climbSettings.lowClimbCheckOffset)).y;
        }
        else if (midClimb) {
            range.low = pivot.localPosition.y + climbSettings.midClimbCheckOffset.y - climbSettings.midClimbRadius + (player.AimDirection() * Vector3.Scale(offsetMask, climbSettings.midClimbCheckOffset)).y;
        }
        else if (highClimb) {
            range.low = pivot.localPosition.y + climbSettings.highClimbCheckOffset.y - climbSettings.highClimbRadius + (player.AimDirection() * Vector3.Scale(offsetMask, climbSettings.midClimbCheckOffset)).y;
        }

        if (highClimb) {
            range.high = pivot.localPosition.y + climbSettings.highClimbCheckOffset.y + climbSettings.highClimbRadius + (player.AimDirection() * Vector3.Scale(offsetMask, climbSettings.lowClimbCheckOffset)).y;
        }
        else if (midClimb) {
            range.high = pivot.localPosition.y + climbSettings.midClimbCheckOffset.y + climbSettings.midClimbRadius + (player.AimDirection() * Vector3.Scale(offsetMask, climbSettings.midClimbCheckOffset)).y;
        }
        else if (lowClimb) {
            range.high = pivot.localPosition.y + climbSettings.lowClimbCheckOffset.y + climbSettings.lowClimbRadius + (player.AimDirection() * Vector3.Scale(offsetMask, climbSettings.highClimbCheckOffset)).y;
        }

        return (lowClimb || midClimb || highClimb);
    }

    private ClimbInfo FindHeight(float minHeight, float maxHeight, float rayLength, float climbableDepth, float charSpeed) {
        Debug.LogFormat("minHeight={0}, maxHeight={1}, rayLength={2}, climableDepth={3}, charSpeed={4}", minHeight, maxHeight, rayLength, climbableDepth, charSpeed);

        Quaternion movementDirection;
        //if (Vector3.ProjectOnPlane(rigidbody.velocity, Vector3.up).sqrMagnitude < 0.001f) {
            movementDirection = player.AimYawQuaternion();
        //}
        //else {
        //    movementDirection = Quaternion.FromToRotation(new Vector3(-rigidbody.velocity.x, 0f, rigidbody.velocity.z).normalized, Vector3.forward);
        //    movementDirection = Quaternion.Slerp(movementDirection, rigidbody.rotation, 0.5f);
        //}

        float heightStep = (maxHeight - minHeight) / numberRaycasts;

        Vector3[] candidatePoints = new Vector3[numberRaycasts];
        Vector3[] candidateNormals = new Vector3[numberRaycasts];
        int highestSuccessfulIndex = 0;
        float lastRaycastDistance = -1.0f;
        RaycastHit currentRaycastHit;
        RaycastHit lastHit = new RaycastHit();

        for (int n = 0; n < numberRaycasts; ++n) {
            Debug.DrawRay(rigidbody.position + movementDirection * ((minHeight + n * heightStep) * Vector3.up + 0.25f * Vector3.back), rayLength * (movementDirection * Vector3.forward), Color.blue, 20f, false);
            if (Physics.SphereCast(rigidbody.position + movementDirection * ((minHeight + n * heightStep) * Vector3.up + 0.25f * Vector3.back),
                    0.5f * heightStep,
                    movementDirection * Vector3.forward,
                    out currentRaycastHit,
                    rayLength,
                    player.raycastMask)) {
                Debug.DrawRay(currentRaycastHit.point, 0.15f * currentRaycastHit.normal, Color.green, 20f, false);
                lastHit = currentRaycastHit;

                if (lastRaycastDistance >= 0.0f && currentRaycastHit.distance - lastRaycastDistance != 0.0f) {
                    float surfaceAngleRad = Mathf.Atan2(heightStep, (currentRaycastHit.distance - lastRaycastDistance));
                    Debug.Log(surfaceAngleRad * Mathf.Rad2Deg);

                    if (surfaceAngleRad > 0f && surfaceAngleRad * Mathf.Rad2Deg < climbableSlopeAngle) {
                        candidatePoints[highestSuccessfulIndex] = currentRaycastHit.point;
                        candidateNormals[highestSuccessfulIndex] = currentRaycastHit.normal;
                        Debug.DrawRay(currentRaycastHit.point, 0.04f * currentRaycastHit.normal, Color.red, 20f, false);
                        ++highestSuccessfulIndex;
                    }
                }
                lastRaycastDistance = currentRaycastHit.distance;
            }
            else {
                if (lastRaycastDistance > 0.0f) {
                    candidatePoints[highestSuccessfulIndex] = lastHit.point;
                    candidateNormals[highestSuccessfulIndex] = lastHit.normal;
                    Debug.DrawRay(lastHit.point, 0.04f * lastHit.normal, Color.red, 20f, false);
                    ++highestSuccessfulIndex;
                }
                lastRaycastDistance = -1.0f;
            }
        }

        Debug.Log(highestSuccessfulIndex);
        Debug.Log(candidatePoints.ToString());

        int finalPointIndex = 0;
        Vector3[] finalClimbCandidatePoints = new Vector3[highestSuccessfulIndex];
        Vector3[] finalClimbCandidatePointsNormals = new Vector3[highestSuccessfulIndex];
        for (int n = 0; n < highestSuccessfulIndex; ++n) {
            Debug.DrawRay(candidatePoints[n] + heightStep * Vector3.up + movementDirection * (Vector3.forward * 0.04f), 2.0f * heightStep * Vector3.down, Color.cyan, 20f, false);
            Debug.Log("Rb.pos=" + rigidbody.position.y + ", maxClimbHeight=" + climbSettings.maxClimbHeight.Evaluate(charSpeed) + " < candidatePoints[" + n + "].y=" + candidatePoints[n].y);
            if (rigidbody.position.y + climbSettings.maxClimbHeight.Evaluate(charSpeed) > candidatePoints[n].y) {
                Debug.DrawRay(candidatePoints[n] + heightStep * Vector3.up + movementDirection * (Vector3.forward * 0.04f), 2.0f * heightStep * Vector3.down, Color.cyan, 20f, false);
                RaycastHit topCollider;
                if (Physics.Raycast(
                            candidatePoints[n] + heightStep * Vector3.up + movementDirection * (Vector3.forward * 0.04f),
                            //0.02f,
                            Vector3.down,
                            out topCollider,
                            2.0f * heightStep,
                            player.raycastMask)) {
                    finalClimbCandidatePoints[finalPointIndex] = topCollider.point;
                    finalClimbCandidatePointsNormals[finalPointIndex] = candidateNormals[n];
                    ++finalPointIndex;
                }
            }
        }

        return new ClimbInfo {
            candidatePointNum = finalPointIndex,
            candidatePoints = finalClimbCandidatePoints,
            candidatePointsNormals = finalClimbCandidatePointsNormals,
        };
    }

    private bool OverlapCapsuleInFront(float radius, float length, Vector3 offset) {
        int collidersSize = Physics.OverlapCapsuleNonAlloc(
            rigidbody.position + pivot.localPosition + (offset.y * Vector3.up) + player.AimDirection() * (Vector3.Scale(offsetMask, offset) + (length * 0.5f * Vector3.left)),
            rigidbody.position + pivot.localPosition + (offset.y * Vector3.up) + player.AimDirection() * (Vector3.Scale(offsetMask, offset) + (length * 0.5f * Vector3.right)),
            radius, overlapBuffer, player.raycastMask);

        return (collidersSize != 0);
    }

    public bool drawGizmo = false;
    public void OnDrawGizmos() {
        if (drawGizmo && rigidbody != null) {
            Quaternion movementDirection = player.AimDirection();
            Gizmos.color = Color.grey;
            Gizmos.DrawWireSphere(rigidbody.position + pivot.localPosition + (climbSettings.lowClimbCheckOffset.y * Vector3.up) + movementDirection * (Vector3.Scale(offsetMask, climbSettings.lowClimbCheckOffset) + (climbSettings.lowClimbLength * 0.5f * Vector3.left)),
                climbSettings.lowClimbRadius);
            Gizmos.DrawWireSphere(rigidbody.position + pivot.localPosition + (climbSettings.lowClimbCheckOffset.y * Vector3.up) + movementDirection * (Vector3.Scale(offsetMask, climbSettings.lowClimbCheckOffset) + (climbSettings.lowClimbLength * 0.5f * Vector3.right)),
                climbSettings.lowClimbRadius);

            Gizmos.DrawWireSphere(rigidbody.position + pivot.localPosition + (climbSettings.midClimbCheckOffset.y * Vector3.up) + movementDirection * (Vector3.Scale(offsetMask, climbSettings.midClimbCheckOffset) + (climbSettings.midClimbLength * 0.5f * Vector3.left)),
                climbSettings.midClimbRadius);
            Gizmos.DrawWireSphere(rigidbody.position + pivot.localPosition + (climbSettings.midClimbCheckOffset.y * Vector3.up) + movementDirection * (Vector3.Scale(offsetMask, climbSettings.midClimbCheckOffset) + (climbSettings.midClimbLength * 0.5f * Vector3.right)),
                climbSettings.midClimbRadius);

            Gizmos.DrawWireSphere(rigidbody.position + pivot.localPosition + (climbSettings.highClimbCheckOffset.y * Vector3.up) + movementDirection * (Vector3.Scale(offsetMask, climbSettings.highClimbCheckOffset) + (climbSettings.highClimbLength * 0.5f * Vector3.left)),
                climbSettings.highClimbRadius);
            Gizmos.DrawWireSphere(rigidbody.position + pivot.localPosition + (climbSettings.highClimbCheckOffset.y * Vector3.up) + movementDirection * (Vector3.Scale(offsetMask, climbSettings.highClimbCheckOffset) + (climbSettings.highClimbLength * 0.5f * Vector3.right)),
                climbSettings.highClimbRadius);
        }

        if (climbPoint != Vector3.zero) {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(climbPoint, 0.04f);
        }
    }

    public bool ValidateClimbAttempt() {
        Range range;
        if (!CheckClimbVolume(out range)) {
            this.climbValid = false;
            return this.climbValid;
        }

        ClimbInfo climbInfo = FindHeight(
            range.low,
            range.high,
            0.25f + climbSettings.lowClimbCheckOffset.z + climbSettings.midClimbRadius,
            climbableDepth,
            rigidbody.velocity.magnitude
        );

        if (climbInfo.candidatePointNum == 0) {
            this.climbValid = false;
            return this.climbValid;
        }

        Debug.LogFormat("Rigidbody.pos = {0}, climbPoint = {1}", rigidbody.position.ToString("F4"), climbInfo.candidatePoints[climbInfo.candidatePointNum - 1].ToString("F4"));

        float verticalDiff = climbInfo.candidatePoints[climbInfo.candidatePointNum - 1].y - rigidbody.position.y;
        Debug.Log("climb vertical diff = " + verticalDiff);
        if (verticalDiff > pivot.localPosition.y + climbSettings.highClimbCheckOffset.y - climbSettings.highClimbRadius) {
            this.animation = ClimbAnimation.High;
        }
        else if (verticalDiff > pivot.localPosition.y + climbSettings.midClimbCheckOffset.y - climbSettings.midClimbRadius) {
            this.animation = ClimbAnimation.Mid;
        }
        else if (verticalDiff > pivot.localPosition.y + climbSettings.lowClimbCheckOffset.y - climbSettings.lowClimbRadius) {
            this.animation = ClimbAnimation.Low;
        }
        else if (verticalDiff > 0.0f) {
            this.animation = ClimbAnimation.Step;
        }
        else {
            this.animation = ClimbAnimation.None;
        }

        if (this.animation == ClimbAnimation.None) {
            this.climbPoint = Vector3.zero;
            this.climbValid = false;
            return this.climbValid;
        }

        this.climbPoint = climbInfo.candidatePoints[climbInfo.candidatePointNum - 1];
        this.climbValid = true;
        return this.climbValid;
    }
}
