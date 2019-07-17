using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Punch : MonoBehaviour
{


    void OnTriggerEnter(Collider hit_obj)
    {
        if(hit_obj.tag == "Player")
        {
            Debug.Log("It hit the player");
        }

    }
}
