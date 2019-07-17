using System.Collections;
using System.Collections.Generic;
using PlayerControl;
using UnityEngine;

public class Punch : MonoBehaviour
{


    void OnTriggerEnter(Collider hit_obj)
    {
        if (hit_obj.gameObject.layer == LayerMask.NameToLayer("Player Character"))
        {
            Debug.Log("It hit the player");
            EventManager.TriggerEvent<PlayerDamageEvent,int>(1);
        }

    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("exiting the pubch collider");
    }
}
