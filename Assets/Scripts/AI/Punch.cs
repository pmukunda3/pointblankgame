using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Punch : MonoBehaviour
{


    void OnTriggerEnter(Collider hit_obj)
    {
        Debug.Log("Hitting something");
        Debug.Log(hit_obj);
        if (hit_obj.gameObject.layer == LayerMask.NameToLayer("Player Character"))
        {
            Debug.Log("It hit the player");
        }

    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("exiting the pubch collider");
    }
}
