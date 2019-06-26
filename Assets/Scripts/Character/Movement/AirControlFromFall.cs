using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirControlFromFall : MonoBehaviour, IMovementState {

    public Vector3 airAccel;

    public MovementChange CalculateAcceleration(Vector2 input, Vector3 localVelocity, float timeStep) {
        return new MovementChange(Vector3.Scale(airAccel, new Vector3(input.x, 0f, input.y)), localVelocity);
    }

    public float MaxSpeed(int direction) {
        throw new System.NotImplementedException();
    }
}
