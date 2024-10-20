using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class Boss_dive : StateMachineBehaviour
{
    Rigidbody2D rigibody2D;
    bool callOnce = false;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        rigibody2D = animator.GetComponent<Rigidbody2D>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        TheHollowKnight.Instance.divingCollider.SetActive(true);

        if (TheHollowKnight.Instance.isGrounded())
        {
            TheHollowKnight.Instance.divingCollider.SetActive(false);
             if (!callOnce)
            {
                TheHollowKnight.Instance.DivingPillars();
                animator.SetBool("dive", false);
                TheHollowKnight.Instance.ResetAllAttacks();
                
                callOnce = true;
            }
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        callOnce = false;
    }
}
