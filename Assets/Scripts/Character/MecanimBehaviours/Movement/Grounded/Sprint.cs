using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerControl {
    namespace MecanimBehaviour {
        public class Sprint : PlayerControllerStateBehaviourBase {

            public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex) {
                base.OnStateEnter(animator, animatorStateInfo, layerIndex);

                Debug.Log("On State Enter: Sprint");
                playerController.SetState(StateId.Player.MoveModes.Grounded.sprint);

                playerController.weaponController.aimingWeapon = false;
            }
        }
    }
}