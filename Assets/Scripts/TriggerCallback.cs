using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TriggerCallback : MonoBehaviour {

    public delegate void TriggerCall(GameObject triggerRef, Collider other);

    private TriggerCall cbEnter = null;
    private TriggerCall cbExit = null;

    public void PassCallback(TriggerCall cbEnter, TriggerCall cbExit = null) {
        this.cbEnter = cbEnter;
        this.cbExit = cbExit;
    }

    void OnTriggerEnter(Collider other) {
        cbEnter?.Invoke(gameObject, other);
    }

    void OnTriggerExit(Collider other) {
        cbExit?.Invoke(gameObject, other);
    }
}
