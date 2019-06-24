using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerControl {
    namespace MecanimBehaviour {
        public class PlayerControllerStateBehaviourBase : StateMachineBehaviour {

            protected PlayerController playerController;

            public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex) {
                playerController = animator.GetComponent<PlayerController>();
            }
        }
    }
}
