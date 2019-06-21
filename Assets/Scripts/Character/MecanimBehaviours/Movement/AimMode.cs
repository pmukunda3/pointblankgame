using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimMode : MonoBehaviour {

    public AnimationEvent onEnter;
    public AnimationEvent onExit;

    public AnimationEvent[] events;

    private float startTime = 0f;

    public void Start() {
        onEnter = new AnimationEvent();
        onExit = new AnimationEvent();
    }
}
