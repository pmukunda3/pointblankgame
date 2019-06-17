using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirControlFromJump : MonoBehaviour, IMovementState {

    public Vector3 airAccel;
    public Vector3 localVelocityLimit;

    public MovementChange CalculateAcceleration(Vector2 input, Vector3 localVelocity, float timeStep) {
        Vector3 acceleration = Vector3.Scale(airAccel, new Vector3(input.x, 0f, input.y));
        if (Mathf.Abs(localVelocity.x) > localVelocityLimit.x) {
            acceleration.x = 0f;
        }
        if (Mathf.Abs(localVelocity.z) > localVelocityLimit.z) {
            acceleration.z = 0f;
        }
        return new MovementChange(acceleration, localVelocity);
    }

    public float MaxSpeed(int direction) {
        throw new System.NotImplementedException();
    }
}
