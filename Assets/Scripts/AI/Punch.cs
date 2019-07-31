using System.Collections;
using System.Collections.Generic;
using PlayerControl;
using UnityEngine;

public class Punch : MonoBehaviour
{
    private bool hitAlready = false;
    private Animator ai_animator;

    private void Start()
    {
        ai_animator = gameObject.GetComponentInParent<Animator>();

    }
    void OnTriggerEnter(Collider hit_obj)
    {

        if (hit_obj.gameObject.layer == LayerMask.NameToLayer("Player Character")&&
        ai_animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            if (!hitAlready)
            {
                Debug.Log("It hit the player");
                EventManager.TriggerEvent<PlayerDamageEvent, int>(1);
                hitAlready = true;
            }

        }

    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("exiting the pubch collider");
        hitAlready = false;
    }
}
