using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerControl;

namespace CameraControl {
    namespace State {
        public class FreeRoam : CameraState {

            public Vector3 offset;

            public float maxSpeed;
            public float maxTurnSpeed;

            public AnimationCurve offsetFuncX;
            public AnimationCurve offsetFuncY;
            public AnimationCurve offsetFuncZ;

            private ThirdPersonCamera thirdPCamera;
            private PlayerController player;

            private Vector3 previousPosition;
            private Quaternion previousRotation;

            public void Start() {
                thirdPCamera = gameObject.GetComponentInParent<ThirdPersonCamera>();
                player = thirdPCamera.player;

                thirdPCamera.RegisterState(StateId.Camera.Grounded.freeRoam, this);

                EventManager.StartListening<PlayerControl.MecanimBehaviour.FreeRoamEvent>(new UnityEngine.Events.UnityAction(OnFreeRoamEvent));
            }

            public override void CameraUpdate() {
                Vector3 pitchAdjustedOffset = new Vector3(
                    offsetFuncX.Evaluate(player.AimPitch() / 90.0f) * offset.x,
                    offsetFuncY.Evaluate(player.AimPitch() / 90.0f) * offset.y,
                    offsetFuncZ.Evaluate(player.AimPitch() / 90.0f) * offset.z);
                Vector3 desiredLocation = thirdPCamera.cameraPivot.transform.position + player.AimDirection() * pitchAdjustedOffset;

                previousPosition = thirdPCamera.transform.position;
                previousRotation = thirdPCamera.transform.rotation;

                    // This ultimately proved to be a really bad way to move the camera
                float angleDiff = Quaternion.Angle(previousRotation, player.AimDirection());
                float distance = Vector3.Distance(previousPosition, desiredLocation);

                float slerpT;
                float lerpT;

                if (angleDiff > maxTurnSpeed * Time.deltaTime) {
                    slerpT = maxTurnSpeed * Time.deltaTime / angleDiff;
                }
                else {
                    slerpT = 1.0f;
                }

                if (distance > maxSpeed * Time.deltaTime) {
                    lerpT = maxSpeed * Time.deltaTime / distance;
                }
                else {
                    lerpT = 1.0f;
                }

                thirdPCamera.transform.rotation = Quaternion.SlerpUnclamped(previousRotation, player.AimDirection(), 0.5f);
                thirdPCamera.transform.position = Vector3.LerpUnclamped(previousPosition, desiredLocation, 0.5f);
            }

            private void OnFreeRoamEvent() {
                previousPosition = thirdPCamera.transform.position;
                previousRotation = thirdPCamera.transform.rotation;

                thirdPCamera.SetState(StateId.Camera.Grounded.freeRoam);
            }
        }
    }
}