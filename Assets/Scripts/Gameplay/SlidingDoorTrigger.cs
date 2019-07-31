using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidingDoorTrigger : MonoBehaviour {
    public Animator animator;

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer != LayerMask.NameToLayer("Static Level Geometry")) {
            animator.SetInteger("ObjectCloseCount", animator.GetInteger("ObjectCloseCount") + 1);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject.layer != LayerMask.NameToLayer("Static Level Geometry")) {
            animator.SetInteger("ObjectCloseCount", animator.GetInteger("ObjectCloseCount") - 1);
        }
    }
}
