using UnityEngine;

public interface IPlayerAim {
    Quaternion AimDirection();
    float AimPitch();
    float AimYaw();
    Quaternion AimYawQuaternion();
}
