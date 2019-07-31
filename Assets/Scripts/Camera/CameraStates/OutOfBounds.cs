using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CameraControl {
    namespace State {
        public class OutOfBounds : CameraState {
            private ThirdPersonCamera thirdPCamera;
            private Rigidbody playerRb;
            private Vector3 enterStatePosition;

            public void Start() {
                thirdPCamera = gameObject.GetComponentInParent<ThirdPersonCamera>();
                playerRb = thirdPCamera.player.GetComponent<Rigidbody>();

                thirdPCamera.RegisterState(StateId.Camera.Falling.outOfBounds, this);

                EventManager.StartListening<PlayerControl.PlayerOutOfBoundsEvent>(new UnityEngine.Events.UnityAction(OnOutOfBounds));
            }

            public override void CameraUpdate() {
                // do nothing
            }

            public override void CameraLateUpdate() {
                //thirdPCamera.transform.position = enterStatePosition;

                thirdPCamera.transform.LookAt(playerRb.position);
            }

            private void OnOutOfBounds() {
                enterStatePosition = thirdPCamera.transform.position;

                thirdPCamera.SetState(StateId.Camera.Falling.outOfBounds);
            }
        }
    }
}