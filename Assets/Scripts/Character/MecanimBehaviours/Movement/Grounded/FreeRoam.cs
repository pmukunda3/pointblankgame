using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerControl {
    namespace MecanimBehaviour {
        public class FreeRoam : PlayerControllerStateBehaviourBase {

            public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex) {
                base.OnStateEnter(animator, animatorStateInfo, layerIndex);

                Debug.Log("On State Enter: FreeRoam");
                EventManager.TriggerEvent<FreeRoamEvent>();
            }
        }

        public class FreeRoamEvent : UnityEngine.Events.UnityEvent { }
    }
}
