using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerControl {
    namespace MecanimBehaviour {
        public class Aiming : PlayerControllerStateBehaviourBase {

            public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex) {
                base.OnStateEnter(animator, animatorStateInfo, layerIndex);

                EventManager.TriggerEvent<AimingEvent>();
            }
        }

        public class AimingEvent : UnityEngine.Events.UnityEvent { }
    }
}