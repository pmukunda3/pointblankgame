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

            private ThirdPersonCamera thirdPCamera;
            private PlayerController player;

            private Vector3 previousPosition;
            private Quaternion previousRotation;

            public void Start() {
                thirdPCamera = gameObject.GetComponentInParent<ThirdPersonCamera>();
                player = thirdPCamera.player;

                thirdPCamera.RegisterState(StateId.Camera.Grounded.sprint, this);

                EventManager.StartListening<PlayerControl.MecanimBehaviour.SprintEvent>(new UnityEngine.Events.UnityAction(OnSprintEvent));
            }

            public override void CameraUpdate() {
                Vector3 pitchAdjustedOffset = new Vector3(
                    offsetFuncX.Evaluate(player.AimPitch() / 90.0f) * offset.x,
                    offsetFuncY.Evaluate(player.AimPitch() / 90.0f) * offset.y,
                    offsetFuncZ.Evaluate(player.AimPitch() / 90.0f) * offset.z);
                Vector3 desiredLocation = thirdPCamera.cameraPivot.transform.position + player.AimDirection() * pitchAdjustedOffset;

                previousPosition = thirdPCamera.transform.position;
                previousRotation = thirdPCamera.transform.rotation;

                thirdPCamera.transform.rotation = Quaternion.SlerpUnclamped(previousRotation, player.AimDirection(), cameraDampTime * Time.deltaTime);
                thirdPCamera.transform.position = Vector3.LerpUnclamped(previousPosition, desiredLocation, cameraDampTime * Time.deltaTime);
            }

            private void OnSprintEvent() {
                thirdPCamera.SetState(StateId.Camera.Grounded.sprint);
            }
        }
    }
}