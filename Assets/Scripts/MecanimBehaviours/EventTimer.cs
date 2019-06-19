using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventTimer : StateMachineBehaviour {

    public UnityEvent unityEvent;
    public UnityEvent[] unityEvents;

    public float time = 0f;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex) {
        time = 0f;
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex) {
        time = 10f;
    }
}
