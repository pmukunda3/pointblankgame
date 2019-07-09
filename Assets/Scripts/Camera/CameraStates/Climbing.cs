using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerControl;

namespace CameraControl {
    namespace State {
        public class Climbing : CameraState {

            public Vector3 offset;

            public float enterStateTime = 0.25f;

            public AnimationCurve offsetFuncX;
            public AnimationCurve offsetFuncY;
            public AnimationCurve offsetFuncZ;

            public AnimationCurve offsetFuncPitch;

            public float cameraDampTime = 0.8f;

            public Vector3 target {
                get;
                private set;
            }

            private ThirdPersonCamera thirdPCamera;
            private PlayerController player;

            private float minDistance;

            private Vector3 previousPosition;
            private Quaternion previousRotation;

            public void Start() {
                thirdPCamera = gameObject.GetComponentInParent<ThirdPersonCamera>();
                player = thirdPCamera.player;

                thirdPCamera.RegisterState(StateId.Camera.Climbing.ledgeClimb, this);

                EventManager.StartListening<PlayerControl.MecanimBehaviour.MidAirToClimbEvent>(new UnityEngine.Events.UnityAction(OnClimbingEvent));
            }

            public override void CameraUpdate() {
                // do nothing
            }

            public override void CameraLateUpdate() {
                Vector3 pitchAdjustedOffset = new Vector3(
                    offsetFuncX.Evaluate(player.AimPitch() / 90.0f) * offset.x,
                    offsetFuncY.Evaluate(player.AimPitch() / 90.0f) * offset.y,
                    offsetFuncZ.Evaluate(player.AimPitch() / 90.0f) * offset.z);
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

                thirdPCamera.transform.rotation = Quaternion.SlerpUnclamped(previousRotation, player.AimDirection() * Quaternion.Euler(-offsetFuncPitch.Evaluate(player.AimPitch() / 90f), 0f, 0f), cameraDampTime * Time.deltaTime);
                thirdPCamera.transform.position = newPosition;
            }

            private void OnClimbingEvent() {
                thirdPCamera.SetState(StateId.Camera.Climbing.ledgeClimb);
            }
        }
    }
}