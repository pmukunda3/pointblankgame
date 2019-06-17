using UnityEngine;

[System.Serializable]
public struct MovementChange {
    public Vector3 localAcceleration;
    public Vector3 localVelocityOverride;

    public MovementChange(Vector3 accel, Vector3 velOver) {
        localAcceleration = accel;
        localVelocityOverride = velOver;
    }
}

public interface IMovementState {
    MovementChange CalculateAcceleration(Vector3 input, Vector3 localVelocity);
    float MaxSpeed(int direction);
}