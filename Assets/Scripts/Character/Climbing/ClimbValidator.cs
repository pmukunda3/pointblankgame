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

    public enum ClimbAnimation : int {
        None = 0,
        Step,
        Low,
        Mid,
        High,
    }

    public ClimbCapsules climbSettings;

    public int numberRaycasts;

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
    }

    public bool GetClimbInfo(out Vector3 climbPoint, out ClimbAnimation animation) {
        if (climbValid) {
            climbPoint = this.climbPoint;
            animation = this.animation;
            this.climbValid = false;
            return true;
        }
        else {
            climbPoint = Vector3.zero;
            animation = ClimbAnimation.None;
            return false;
        }
    }

    private ClimbAnimation CheckClimbVolume() {
        bool lowClimb  = OverlapCapsuleInFront(climbSettings.lowClimbRadius, climbSettings.lowClimbLength, climbSettings.lowClimbCheckOffset);
        bool midClimb  = OverlapCapsuleInFront(climbSettings.midClimbRadius, climbSettings.midClimbLength, climbSettings.midClimbCheckOffset);
        bool highClimb = OverlapCapsuleInFront(climbSettings.highClimbRadius, climbSettings.highClimbLength, climbSettings.highClimbCheckOffset);

        if (lowClimb) {
            if (midClimb) {
                if (highClimb) {
                    return ClimbAnimation.High;
                }
                else {
                    return ClimbAnimation.Mid;
                }
            }
            else {
                if (highClimb) {
                    return ClimbAnimation.High;
                }
                else {
                    return ClimbAnimation.Low;
                }
            }
        }
        else {
            if (midClimb) {
                if (highClimb) {
                    return ClimbAnimation.High;
                }
                else {
                    return ClimbAnimation.Mid;
                }
            }
            else {
                if (highClimb) {
                    return ClimbAnimation.High;
                }
                else {
                    return ClimbAnimation.None;
                }
            }
        }
    }

    private Vector3 FindHeight(float minHeight, float maxHeight, float rayLength, float charSpeed) {
        RaycastHit highestSuccessful = new RaycastHit();
        bool foundRaycast = false;

        Quaternion movementDirection;
        if (Vector3.ProjectOnPlane(rigidbody.velocity, Vector3.up).sqrMagnitude < 0.001f) {
            movementDirection = rigidbody.rotation;
        }
        else {
            movementDirection = Quaternion.FromToRotation(new Vector3(-rigidbody.velocity.x, 0f, rigidbody.velocity.z).normalized, Vector3.forward);
            movementDirection = Quaternion.Slerp(movementDirection, rigidbody.rotation, 0.5f);
        }

        float heightStep = (maxHeight - minHeight) / numberRaycasts;

        int n;
        RaycastHit currentRaycastHit;
        for (n = 0; n < numberRaycasts; ++n) {
            if (!Physics.Raycast(rigidbody.position + (minHeight + n * heightStep) * Vector3.up,
                movementDirection * Vector3.forward,
                out currentRaycastHit,
                rayLength,
                player.raycastMask)) {
                if (foundRaycast) {
                    break;
                }
            }
            else {
                foundRaycast = true;
                highestSuccessful = currentRaycastHit;
            }
        }

        if (!foundRaycast && n == numberRaycasts) {
            return Vector3.zero;
        }

        if (rigidbody.position.y + climbSettings.maxClimbHeight.Evaluate(charSpeed) > highestSuccessful.point.y) {
            Debug.DrawRay(highestSuccessful.point + (n * heightStep * Vector3.up) + movementDirection * (Vector3.forward * 0.04f), 2.0f * heightStep * Vector3.down, Color.cyan, 20f, false);

            RaycastHit topCollider;
            //if (Physics.SphereCast(
            //        highestSuccessful.point + (n * heightStep * Vector3.up) + movementDirection * (Vector3.forward * 0.04f),
            //        0.15f,
            //        Vector3.down,
            //        out topCollider,
            //        2.0f * heightStep,
            //        player.raycastMask)) {
            //    GameObject sphere0 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            //    sphere0.transform.position = highestSuccessful.point;
            //    sphere0.transform.localScale *= 0.05f;
            //    return topCollider.point;
            //}
            //else {
            //    return Vector3.zero;
            //}

            if (Physics.Raycast(
                        highestSuccessful.point + (n * heightStep * Vector3.up) + movementDirection * (Vector3.forward * 0.04f),
                        //0.15f,
                        Vector3.down,
                        out topCollider,
                        2.0f * heightStep,
                        player.raycastMask)) {
                return topCollider.point;
            }
            else {
                return Vector3.zero;
            }
        }
        else {
            return Vector3.zero;
        }
    }

    private bool OverlapCapsuleInFront(float radius, float length, Vector3 offset) {
        int collidersSize = Physics.OverlapCapsuleNonAlloc(rigidbody.position + rigidbody.rotation * (offset + (length * 0.5f * Vector3.left)),
            rigidbody.position + rigidbody.rotation * (offset + (length * 0.5f * Vector3.right)),
            radius, overlapBuffer, player.raycastMask);

        return (collidersSize != 0);
    }

    public bool drawGizmo = false;
    public void OnDrawGizmos() {
        if (drawGizmo && rigidbody != null) {
            Gizmos.color = Color.grey;
            Gizmos.DrawWireSphere(rigidbody.position + rigidbody.rotation * (climbSettings.lowClimbCheckOffset + (climbSettings.lowClimbLength * 0.5f * Vector3.left)),
                climbSettings.lowClimbRadius);
            Gizmos.DrawWireSphere(rigidbody.position + rigidbody.rotation * (climbSettings.lowClimbCheckOffset + (climbSettings.lowClimbLength * 0.5f * Vector3.right)),
                climbSettings.lowClimbRadius);

            Gizmos.DrawWireSphere(rigidbody.position + rigidbody.rotation * (climbSettings.midClimbCheckOffset + (climbSettings.midClimbLength * 0.5f * Vector3.left)),
                climbSettings.midClimbRadius);
            Gizmos.DrawWireSphere(rigidbody.position + rigidbody.rotation * (climbSettings.midClimbCheckOffset + (climbSettings.midClimbLength * 0.5f * Vector3.right)),
                climbSettings.midClimbRadius);

            Gizmos.DrawWireSphere(rigidbody.position + rigidbody.rotation * (climbSettings.highClimbCheckOffset + (climbSettings.highClimbLength * 0.5f * Vector3.left)),
                climbSettings.highClimbRadius);
            Gizmos.DrawWireSphere(rigidbody.position + rigidbody.rotation * (climbSettings.highClimbCheckOffset + (climbSettings.highClimbLength * 0.5f * Vector3.right)),
                climbSettings.highClimbRadius);
        }
    }

    public bool ValidateClimbAttempt() {
        int climbVolume = (int) CheckClimbVolume();

        if (climbVolume == (int) ClimbAnimation.None) {
            this.climbValid = false;
            return this.climbValid;
        }

        Vector3 climbPoint = Vector3.zero;
        switch (climbVolume) {
            case (int) ClimbAnimation.Low:
                climbPoint = FindHeight(
                    climbSettings.lowClimbCheckOffset.y - climbSettings.lowClimbRadius,
                    climbSettings.lowClimbCheckOffset.y + climbSettings.lowClimbRadius,
                    climbSettings.lowClimbCheckOffset.z + climbSettings.lowClimbRadius,
                    rigidbody.velocity.magnitude
                );
                break;
            case (int) ClimbAnimation.Mid:
                climbPoint = FindHeight(
                    climbSettings.midClimbCheckOffset.y - climbSettings.midClimbRadius,
                    climbSettings.midClimbCheckOffset.y + climbSettings.midClimbRadius,
                    climbSettings.midClimbCheckOffset.z + climbSettings.midClimbRadius,
                    rigidbody.velocity.magnitude
                );
                break;
            case (int) ClimbAnimation.High:
                climbPoint = FindHeight(
                    climbSettings.highClimbCheckOffset.y - climbSettings.highClimbRadius,
                    climbSettings.highClimbCheckOffset.y + climbSettings.highClimbRadius,
                    climbSettings.highClimbCheckOffset.z + climbSettings.highClimbRadius,
                    rigidbody.velocity.magnitude
                );
                break;
        }

        float verticalDiff = climbPoint.y - rigidbody.position.y;
        Debug.Log("climb vertical diff = " + verticalDiff);
        if (verticalDiff > climbSettings.highClimbCheckOffset.y) {
            this.animation = ClimbAnimation.High;
        }
        else if (verticalDiff > climbSettings.midClimbCheckOffset.y) {
            this.animation = ClimbAnimation.Mid;
        }
        else if (verticalDiff > climbSettings.lowClimbCheckOffset.y) {
            this.animation = ClimbAnimation.Low;
        }
        else {
            this.animation = ClimbAnimation.Step;
        }

        this.climbPoint = climbPoint;
        this.climbValid = true;
        return this.climbValid;
    }
}
