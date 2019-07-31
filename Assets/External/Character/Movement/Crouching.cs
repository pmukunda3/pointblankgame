using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerControl;

public class Crouching : MonoBehaviour, IMovementState {

    public Vector3 groundDecel;

    public MovementChange CalculateAcceleration(Vector2 input, Vector3 localVelocity, float timeStep) {
        float lateralAcceleration = groundDecel.x;
        if (Mathf.Abs(localVelocity.x) + lateralAcceleration * timeStep < PlayerController.EPSILON) {
            lateralAcceleration = localVelocity.x / timeStep;
        }
        lateralAcceleration *= Mathf.Sign(localVelocity.x);

        float forwardAcceleration = groundDecel.z;
        if (Mathf.Abs(localVelocity.z) + forwardAcceleration * timeStep < PlayerController.EPSILON) {
            forwardAcceleration = localVelocity.z / timeStep;
        }
        forwardAcceleration *= Mathf.Sign(localVelocity.z);

        return new MovementChange(new Vector3(lateralAcceleration, 0f, forwardAcceleration), localVelocity);
    }

    public float MaxSpeed(int direction) {
        throw new System.NotImplementedException();
    }
}
