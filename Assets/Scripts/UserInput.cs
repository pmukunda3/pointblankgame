using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInput : MonoBehaviour {

    public KeyCode walkKey;
    public KeyCode sprintKey;
    public KeyCode crouchKey;
    public KeyCode useKey;
    public KeyCode primaryFireKey;
    public KeyCode jumpKey;
    public KeyCode secondaryFireKey;
    public KeyCode reloadKey;
    public KeyCode climbUpKey;
    public KeyCode changeWeaponKey;

    [System.Serializable]
    public struct KeyState {
        public bool down;
        public bool up;
        public bool active;
    }

    [System.Serializable]
    public struct Actions {
        public KeyState walk;
        public KeyState sprint;
        public KeyState crouch;
        public KeyState use;
        public KeyState primaryFire;
        public KeyState jump;
        public KeyState aim;
        public KeyState reload;
        public KeyState climbUp;
        public KeyState grabLedge;
        public KeyState changeWeapon;
    }

    private Actions _actions;

    public Actions actions {
        get { return _actions; }
    }

    void Update() {
        _actions.walk.down = Input.GetKeyDown(walkKey);
        _actions.walk.up = Input.GetKeyUp(walkKey);
        _actions.walk.active = Input.GetKey(walkKey);

        _actions.sprint.down = Input.GetKeyDown(sprintKey);
        _actions.sprint.up = Input.GetKeyUp(sprintKey);
        _actions.sprint.active = Input.GetKey(sprintKey);

        _actions.crouch.down = Input.GetKeyDown(crouchKey);
        _actions.crouch.up = Input.GetKeyUp(crouchKey);
        _actions.crouch.active = Input.GetKey(crouchKey);

        _actions.use.down = Input.GetKeyDown(useKey);
        _actions.use.up = Input.GetKeyUp(useKey);
        _actions.use.active = Input.GetKey(useKey);

        _actions.primaryFire.down = Input.GetKeyDown(primaryFireKey);
        _actions.primaryFire.up = Input.GetKeyUp(primaryFireKey);
        _actions.primaryFire.active = Input.GetKey(primaryFireKey);

        _actions.jump.down = Input.GetKeyDown(jumpKey);
        _actions.jump.up = Input.GetKeyUp(jumpKey);
        _actions.jump.active = Input.GetKey(jumpKey);

        _actions.aim.down = Input.GetKeyDown(secondaryFireKey);
        _actions.aim.up = Input.GetKeyUp(secondaryFireKey);
        _actions.aim.active = Input.GetKey(secondaryFireKey);

        _actions.reload.down = Input.GetKeyDown(reloadKey);
        _actions.reload.up = Input.GetKeyUp(reloadKey);
        _actions.reload.active = Input.GetKey(reloadKey);

        _actions.climbUp.down = Input.GetKeyDown(climbUpKey);
        _actions.climbUp.up = Input.GetKeyUp(climbUpKey);
        _actions.climbUp.active = Input.GetKey(climbUpKey);

        _actions.changeWeapon.down = Input.GetKeyDown(changeWeaponKey);
        _actions.changeWeapon.up = Input.GetKeyUp(changeWeaponKey);
        _actions.changeWeapon.active = Input.GetKey(changeWeaponKey);
    }
}
