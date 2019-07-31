using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerControl {
    namespace MecanimBehaviour {
        public class Crouch : PlayerControllerStateBehaviourBase {

            public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex) {
                base.OnStateEnter(animator, animatorStateInfo, layerIndex);

                EventManager.TriggerEvent<CrouchEvent>();
            }
        }

        public class CrouchEvent : UnityEngine.Events.UnityEvent { }
    }
}
