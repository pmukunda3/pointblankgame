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

            // This creates a pretty neat 3rd person following camera that isn't
            // half bad, but probably not the kind of game I'm going for.
            // public float cameraDampTime = 1.8f;
            public float cameraDampTime = 3.6f;

            public AnimationCurve offsetFuncX;
            public AnimationCurve offsetFuncY;
            public AnimationCurve offsetFuncZ;

            private float minDistance;

            private ThirdPersonCamera thirdPCamera;
            private PlayerController player;

            private Vector3 previousPosition;
            private Quaternion previousRotation;

            public void Start() {
                thirdPCamera = gameObject.GetComponentInParent<ThirdPersonCamera>();
                player = thirdPCamera.player;

                minDistance = Vector3.ProjectOnPlane(offset, Vector3.up).magnitude * 0.9f;

                thirdPCamera.RegisterState(StateId.Camera.Grounded.freeRoam, this);

                EventManager.StartListening<PlayerControl.MecanimBehaviour.FreeRoamEvent>(new UnityEngine.Events.UnityAction(OnFreeRoamEvent));
            }

            public override void CameraUpdate() {
                Vector3 pitchAdjustedOffset = new Vector3(
                    offsetFuncX.Evaluate(player.AimPitch() / 90.0f) * offset.x,
                    offsetFuncY.Evaluate(player.AimPitch() / 90.0f) * offset.y,
                    offsetFuncZ.Evaluate(player.AimPitch() / 90.0f) * offset.z);

                //TODO: project the camera onto the vector behind the player character to maintain a minimum distance away from the player character.
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

                if (Input.GetKeyDown(KeyCode.P)) Debug.Break();
            }

            private void OnFreeRoamEvent() {
                previousPosition = thirdPCamera.transform.position;
                previousRotation = thirdPCamera.transform.rotation;

                thirdPCamera.SetState(StateId.Camera.Grounded.freeRoam);
            }
        }
    }
}