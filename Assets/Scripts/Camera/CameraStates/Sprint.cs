using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerControl;

namespace CameraControl {
    namespace State {
        public class Sprint : CameraState {

            public Vector3 offset;

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
            }

            public override void CameraUpdate() {
                Vector3 pitchAdjustedOffset = new Vector3(
                    offsetFuncX.Evaluate(player.AimPitch() / 90.0f) * offset.x,
                    offsetFuncY.Evaluate(player.AimPitch() / 90.0f) * offset.y,
                    offsetFuncZ.Evaluate(player.AimPitch() / 90.0f) * offset.z);
                Vector3 desiredLocation = thirdPCamera.cameraPivot.transform.position + player.AimDirection() * pitchAdjustedOffset;

                //Vector3 cameraAlignProjection = Vector3.Project(transform.position - desiredLocation, player.AimDirection() * -Vector3.forward);

                previousPosition = thirdPCamera.transform.position;
                previousRotation = thirdPCamera.transform.rotation;

                thirdPCamera.transform.rotation = player.AimDirection() * Quaternion.Euler(2f, 0f, 0f);
                thirdPCamera.transform.position = desiredLocation; // + cameraAlignProjection;
            }
        }
    }
}