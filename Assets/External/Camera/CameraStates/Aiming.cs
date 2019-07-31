using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CameraControl {
    namespace State {
        public class Aiming : CameraState {

            public Vector3 offset;

            public AnimationCurve cameraAimEnterDistance;
            public float enterStateTime = 0.25f;

            public AnimationCurve offsetFuncX;
            public AnimationCurve offsetFuncY;
            public AnimationCurve offsetFuncZ;

            public float aimDistance = 100f;
            public Transform weaponPivot;

            public Vector3 target {
                get;
                private set;
            }

            private ThirdPersonCamera thirdPCamera;

            private Vector3 enterStatePosition;
            private Quaternion enterStateRotation;

            private float elapsedTime = 0.0f;
            private bool enterState = false;

            public void Start() {
                thirdPCamera = gameObject.GetComponentInParent<ThirdPersonCamera>();

                if (weaponPivot == null) weaponPivot = GameObject.FindGameObjectWithTag("weaponPivot").transform;

                thirdPCamera.RegisterState(StateId.Camera.Grounded.aim, this);

                EventManager.StartListening<PlayerControl.MecanimBehaviour.AimingEvent>(new UnityEngine.Events.UnityAction(OnAimingEvent));
            }

            public override void CameraUpdate() {
                SetTarget();
            }

            public override void CameraLateUpdate() {
                Vector3 pitchAdjustedOffset = new Vector3(
                    offsetFuncX.Evaluate(thirdPCamera.player.AimPitch() / 90.0f) * offset.x,
                    offsetFuncY.Evaluate(thirdPCamera.player.AimPitch() / 90.0f) * offset.y,
                    offsetFuncZ.Evaluate(thirdPCamera.player.AimPitch() / 90.0f) * offset.z);
                Vector3 desiredLocation = thirdPCamera.cameraPivot.transform.position + thirdPCamera.player.AimDirection() * pitchAdjustedOffset;

                //Vector3 cameraAlignProjection = Vector3.Project(transform.position - desiredLocation, player.AimDirection() * -Vector3.forward);

                if (enterState) {
                    thirdPCamera.transform.rotation = Quaternion.Slerp(enterStateRotation, thirdPCamera.player.AimDirection() * Quaternion.Euler(2f, 0f, 0f), cameraAimEnterDistance.Evaluate(elapsedTime / enterStateTime));
                    thirdPCamera.transform.position = Vector3.Lerp(enterStatePosition, desiredLocation, cameraAimEnterDistance.Evaluate(elapsedTime / enterStateTime));

                    elapsedTime += Time.deltaTime;
                    if (elapsedTime > enterStateTime) {
                        enterState = false;
                    }
                }
                else {
                    thirdPCamera.transform.rotation = thirdPCamera.player.AimDirection() * Quaternion.Euler(2f, 0f, 0f);
                    thirdPCamera.transform.position = desiredLocation;
                }
            }

            private void SetTarget() {
                if (Physics.Raycast(thirdPCamera.transform.position, thirdPCamera.transform.forward, out RaycastHit hit, aimDistance, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore)) {
                    target = hit.point;
                }
                else {
                    target = thirdPCamera.transform.position + aimDistance * thirdPCamera.transform.forward;
                }
                weaponPivot.LookAt(target);
            }

            private void OnAimingEvent() {
                enterState = true;
                elapsedTime = 0.0f;
                SetTarget();

                enterStatePosition = thirdPCamera.transform.position;
                enterStateRotation = thirdPCamera.transform.rotation;

                thirdPCamera.SetState(StateId.Camera.Grounded.aim);
            }
        }
    }
}