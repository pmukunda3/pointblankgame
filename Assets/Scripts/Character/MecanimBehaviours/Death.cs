using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Death : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        EventManager.TriggerEvent<PlayerDeathEvent>();
    }
}

public class PlayerDeathEvent: UnityEngine.Events.UnityEvent { }