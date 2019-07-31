using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutOfBoundsTrigger : MonoBehaviour {
    public void OnTriggerEnter(Collider other) {

        GameObject curr = other.gameObject;
        bool foundTag = false;
        while (curr != null && !foundTag) {
            if (curr.CompareTag("Player")) {
                EventManager.TriggerEvent<PlayerControl.PlayerOutOfBoundsEvent>();
                foundTag = true;
            }
            if (curr.transform.parent != null) {
                curr = curr.transform.parent.gameObject;
            }
            else {
                curr = null;
            }
        }
    }
}
