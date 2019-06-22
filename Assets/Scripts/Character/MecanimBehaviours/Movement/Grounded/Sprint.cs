using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sprint : PlayerControllerStateBehaviourBase {

    public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex) {
        base.OnStateEnter(animator, animatorStateInfo, layerIndex);

        playerController.SetMovement(PlayerStateId.MoveModes.Grounded.sprint);
    }
}
