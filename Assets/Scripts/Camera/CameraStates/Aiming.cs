using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerControl;

namespace CameraControl {
    namespace State {
        public class Aiming : CameraState {

            public Vector3 offset;

            public AnimationCurve cameraAimEnterDistance;
            public float enterStateTime = 0.25f;

            public AnimationCurve offsetFuncX;
            public AnimationCurve offsetFuncY;
            public AnimationCurve offsetFuncZ;

            private ThirdPersonCamera thirdPCamera;
            private PlayerController player;

            private Vector3 enterStatePosition;
            private Quaternion enterStateRotation;

            private float elapsedTime = 0.0f;
            private bool enterState = false;

            public void Start() {
                thirdPCamera = gameObject.GetComponentInParent<ThirdPersonCamera>();
                player = thirdPCamera.player;

                thirdPCamera.RegisterState(StateId.Camera.Grounded.aim, this);

                EventManager.StartListening<PlayerControl.MecanimBehaviour.AimingEvent>(new UnityEngine.Events.UnityAction(OnAimingEvent));
            }

            public override void CameraUpdate() {
                Vector3 pitchAdjustedOffset = new Vector3(
                    offsetFuncX.Evaluate(player.AimPitch() / 90.0f) * offset.x,
                    offsetFuncY.Evaluate(player.AimPitch() / 90.0f) * offset.y,
                    offsetFuncZ.Evaluate(player.AimPitch() / 90.0f) * offset.z);
                Vector3 desiredLocation = thirdPCamera.cameraPivot.transform.position + player.AimDirection() * pitchAdjustedOffset;

                //Vector3 cameraAlignProjection = Vector3.Project(transform.position - desiredLocation, player.AimDirection() * -Vector3.forward);

                if (enterState) {
                    thirdPCamera.transform.rotation = Quaternion.Slerp(enterStateRotation, player.AimDirection() * Quaternion.Euler(2f, 0f, 0f), cameraAimEnterDistance.Evaluate(elapsedTime / enterStateTime));
                    thirdPCamera.transform.position = Vector3.Lerp(enterStatePosition, desiredLocation, cameraAimEnterDistance.Evaluate(elapsedTime / enterStateTime));

                    elapsedTime += Time.deltaTime;
                    if (elapsedTime > enterStateTime) {
                        enterState = false;
                    }
                }
                else {
                    thirdPCamera.transform.rotation = player.AimDirection() * Quaternion.Euler(2f, 0f, 0f);
                    thirdPCamera.transform.position = desiredLocation;
                }
            }

            private void OnAimingEvent() {
                enterState = true;
                elapsedTime = 0.0f;

                enterStatePosition = thirdPCamera.transform.position;
                enterStateRotation = thirdPCamera.transform.rotation;

                thirdPCamera.SetState(StateId.Camera.Grounded.aim);
            }
        }
    }
}