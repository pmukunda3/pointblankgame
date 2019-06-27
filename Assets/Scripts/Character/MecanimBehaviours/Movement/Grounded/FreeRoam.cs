using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PlayerControl {
    namespace MecanimBehaviour {
        public class FreeRoam : PlayerControllerStateBehaviourBase {

            public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex) {
                base.OnStateEnter(animator, animatorStateInfo, layerIndex);

                Debug.Log("On State Enter: FreeRoam");
                //playerController.SetState(StateId.Player.MoveModes.Grounded.freeRoam);
                //playerController.weaponController.aimingWeapon = false;

                EventManager.TriggerEvent<FreeRoamEvent>();
            }
        }

        public class FreeRoamEvent : UnityEvent { }
    }
}
