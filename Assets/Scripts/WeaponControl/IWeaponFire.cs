using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeaponFire {
    float GetHeatPerShot();
    void FireWeapon();
    void FireWeaponDown();
    void FireWeaponUp();
}
