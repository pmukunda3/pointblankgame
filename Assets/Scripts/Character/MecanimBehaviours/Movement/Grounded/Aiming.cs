using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PlayerControl {
    namespace MecanimBehaviour {
        public class Aiming : PlayerControllerStateBehaviourBase {

            public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex) {
                base.OnStateEnter(animator, animatorStateInfo, layerIndex);

                Debug.Log("On State Enter: Aiming");
                //playerController.SetState(StateId.Player.MoveModes.Grounded.aiming);
                //playerController.weaponController.aimingWeapon = true;

                EventManager.TriggerEvent<AimingEvent>();

                //EventManager.TriggerEvent<CameraControl.ThirdPersonCamera.ThirdPersonCameraStateEvent, Id>(StateId.Player.MoveModes.Grounded.aiming);
                //EventManager.TriggerEvent<PlayerControl.PlayerController.PlayerControlStateEvent, Id>(StateId.Player.MoveModes.Grounded.aiming);
            }
        }

        public class AimingEvent : UnityEvent { }
    }
}