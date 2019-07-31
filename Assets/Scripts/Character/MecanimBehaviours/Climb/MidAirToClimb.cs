using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

namespace PlayerControl {
    namespace MecanimBehaviour {
        public class MidAirToClimb : PlayerControllerStateBehaviourBase {

            public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex) {
                base.OnStateEnter(animator, animatorStateInfo, layerIndex);

                EventManager.TriggerEvent<MidAirToClimbEvent>();
            }

            public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
                base.OnStateExit(animator, stateInfo, layerIndex);

                EventManager.TriggerEvent<MidAirToClimbEventExit>();
            }
        }

        public class MidAirToClimbEvent : UnityEngine.Events.UnityEvent { }
        public class MidAirToClimbEventExit : UnityEngine.Events.UnityEvent { }
    }
}