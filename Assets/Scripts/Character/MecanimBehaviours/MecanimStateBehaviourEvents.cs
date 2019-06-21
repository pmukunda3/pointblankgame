using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MecanimStateBehaviourEvents : StateMachineBehaviour {

    public AnimationEvent onEnter;
    public AnimationEvent onExit;

    public AnimationEvent[] events;

    private float startTime = 0f;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex) {        
        startTime = Time.time;
        onEnter.time = 1.5f;
        onEnter.functionName = "TestFunction";
        onEnter.intParameter = 12345;
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex) {
        //onExit.Invoke();
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (Time.time > startTime + 5f) {

        }
    }
}
