using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerControl;

namespace CameraControl {
    namespace State {
        public class Sprint : CameraState {

            public Vector3 offset;

            public float cameraDampTime = 0.45f;

            public AnimationCurve offsetFuncX;
            public AnimationCurve offsetFuncY;
            public AnimationCurve offsetFuncZ;

            public AnimationCurve lookDiffFuncX;
            public AnimationCurve lookDiffFuncZ;

            private ThirdPersonCamera thirdPCamera;
            private PlayerController player;

            private float minDistance;

            private Vector3 previousPosition;
            private Quaternion previousRotation;

            public void Start() {
                thirdPCamera = gameObject.GetComponentInParent<ThirdPersonCamera>();
                player = thirdPCamera.player;

                thirdPCamera.RegisterState(StateId.Camera.Grounded.sprint, this);

                minDistance = Vector3.ProjectOnPlane(offset, Vector3.up).magnitude * 0.9f;

                EventManager.StartListening<PlayerControl.MecanimBehaviour.SprintEvent>(new UnityEngine.Events.UnityAction(OnSprintEvent));
            }

            public override void CameraUpdate() {
                // do nothing
            }

            public override void CameraLateUpdate() {
                Vector3 pitchAdjustedOffset = new Vector3(
                    (offsetFuncX.Evaluate(player.AimPitch() / 90.0f) + lookDiffFuncX.Evaluate(player.LookToMoveAngle())) * offset.x,
                    offsetFuncY.Evaluate(player.AimPitch() / 90.0f) * offset.y,
                    (offsetFuncZ.Evaluate(player.AimPitch() / 90.0f) + lookDiffFuncZ.Evaluate(player.LookToMoveAngle())) * offset.z);
                Vector3 desiredLocation = thirdPCamera.cameraPivot.transform.position + player.AimDirection() * pitchAdjustedOffset;

                previousPosition = thirdPCamera.transform.position;
                previousRotation = thirdPCamera.transform.rotation;

                Vector3 newPosition = Vector3.LerpUnclamped(previousPosition, desiredLocation, cameraDampTime * Time.deltaTime);
                Vector3 newPositionFlat = new Vector3(newPosition.x, 0f, newPosition.z);
                Vector3 cameraPositionFlat = new Vector3(thirdPCamera.cameraPivot.transform.position.x, 0f, thirdPCamera.cameraPivot.transform.position.z);

                float cameraPlayerDistance = Vector3.Distance(newPosition, thirdPCamera.cameraPivot.transform.position);
                if (cameraPlayerDistance < minDistance) {
                    newPosition += (((80f - Mathf.Abs(player.AimPitch())) / 80f) * (minDistance - cameraPlayerDistance))
                        * (newPositionFlat - cameraPositionFlat).normalized;
                }

                thirdPCamera.transform.rotation = Quaternion.SlerpUnclamped(previousRotation, player.AimDirection(), cameraDampTime * Time.deltaTime);
                thirdPCamera.transform.position = newPosition;
            }

            private void OnSprintEvent() {
                thirdPCamera.SetState(StateId.Camera.Grounded.sprint);
            }
        }
    }
}