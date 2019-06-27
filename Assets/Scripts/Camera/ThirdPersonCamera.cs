using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CameraControl {
    [RequireComponent(typeof(Camera))]
    public class ThirdPersonCamera : MonoBehaviour {

        private Camera camera;
        public PlayerControl.PlayerController player;

        public GameObject cameraPivot;

        private Dictionary<Id, CameraState> cameraStates = new Dictionary<Id, CameraState>();
        private CameraState currState;
        public CameraState initState;

        void Start() {
            camera = gameObject.GetComponent<Camera>();

            currState = initState;

            //EventManager.StartListening<WeaponFirePrimary>(
            //    new UnityEngine.Events.UnityAction(WeaponFirePrimaryCallbackTest));
            //EventManager.StartListening<WeaponFireSecondary, float>(
            //    new UnityEngine.Events.UnityAction<float>(WeaponFirePrimaryCallbackTest));
        }

        void LateUpdate() {
            currState.CameraUpdate();
        }

        public void SetState(Id stateId) {
            CameraState state;
            if (cameraStates.TryGetValue(stateId, out state)) {
                currState = state;
            }
            else {
                currState = initState;
            }
        }

        public void RegisterState(Id stateId, CameraState state) {
            cameraStates.Add(stateId, state);
        }

        public class ThirdPersonCameraStateEvent : UnityEngine.Events.UnityEvent<Id> { }
    }
}