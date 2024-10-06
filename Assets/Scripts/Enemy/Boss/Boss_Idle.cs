using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Idle : StateMachineBehaviour
{
    Rigidbody2D rigidbody2D;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        rigidbody2D = animator.GetComponentInParent<Rigidbody2D>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       rigidbody2D.velocity = Vector2.zero;
       RunToPlayer(animator);
       if(TheHollowKnight.Instance.attackCountdown <= 0)
       {
            TheHollowKnight.Instance.AttackHandler();
            TheHollowKnight.Instance.attackCountdown = TheHollowKnight.Instance.attackTimer;
       }
    }

    void RunToPlayer(Animator animator){
        if(Vector2.Distance(PlayerMovement.Instance.transform.position, rigidbody2D.position) > TheHollowKnight.Instance.GetAttackRange()){
            animator.SetBool("run",true);
        }else{
            return;
        }
    }
    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
