using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawPivot : MonoBehaviour {

    public Color gizmoColor;

    void OnDrawGizmos() {
        Gizmos.color = gizmoColor;
        Gizmos.DrawSphere(transform.position, 0.08f);
    }
}
