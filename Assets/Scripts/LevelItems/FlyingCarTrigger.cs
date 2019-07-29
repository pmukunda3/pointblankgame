using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FlyingCarTrigger : MonoBehaviour {
    
    public enum TriggerType : int {
        Random,
        Collider
    }

    public TriggerType triggerType;

    public float minTime;
    public float maxTime;
    public Collider trigger;

    private UnityAction callback;
    private bool triggerable = true;

    public void Start() {
        StartTimer();
    }

    public void OnTriggerEnter(Collider other) {
        if (triggerable) {
            callback?.Invoke();
            triggerable = false;
        }
    }

    public void SetCallback(UnityAction callback) {
        this.callback = callback;
    }

    public void StartTimer() {
        switch (triggerType) {
            case TriggerType.Random:
                StartCoroutine(WaitToTrigger(Random.value * maxTime + minTime));
                break;
            case TriggerType.Collider:
                triggerable = true;
                break;
        }
    }

    private IEnumerator WaitToTrigger(float time) {
        yield return new WaitForSeconds(time);

        callback?.Invoke();
    }
}
