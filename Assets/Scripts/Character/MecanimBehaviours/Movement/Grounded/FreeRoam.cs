using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerControl {
    namespace MecanimBehaviour {
        public class FreeRoam : PlayerControllerStateBehaviourBase {

            public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex) {
                base.OnStateEnter(animator, animatorStateInfo, layerIndex);

                Debug.Log("On State Enter: FreeRoam");
                playerController.SetState(PlayerStateId.MoveModes.Grounded.freeRoam);

                playerController.weaponController.aimingWeapon = false;
            }
        }
    }
}
