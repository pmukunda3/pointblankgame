using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerControl {
    namespace MecanimBehaviour {
        public class Falling : PlayerControllerStateBehaviourBase {

            public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex) {
                base.OnStateEnter(animator, animatorStateInfo, layerIndex);

                Debug.Log("On State Enter: Falling");
                playerController.SetState(PlayerStateId.MoveModes.Air.falling);

                playerController.weaponController.aimingWeapon = false;
            }
        }
    }
}
