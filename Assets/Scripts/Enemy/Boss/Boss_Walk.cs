using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Walk : StateMachineBehaviour
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
        TargetPlayerPosition(animator);
        if (TheHollowKnight.Instance.attackCountdown <= 0)
        {
            TheHollowKnight.Instance.AttackHandler();
            TheHollowKnight.Instance.attackCountdown = TheHollowKnight.Instance.attackTimer;
        }
    }

    void TargetPlayerPosition(Animator animator)
    {
        if (TheHollowKnight.Instance.isGrounded())
        {
            TheHollowKnight.Instance.Flip();
            Vector2 _target = new Vector2(PlayerMovement.Instance.transform.position.x, rigidbody2D.position.y);
            Vector2 _newPos = Vector2.MoveTowards(rigidbody2D.position, _target, TheHollowKnight.Instance.runSpeed * Time.fixedDeltaTime);
            TheHollowKnight.Instance.runSpeed = TheHollowKnight.Instance.speed;
            rigidbody2D.MovePosition(_newPos);
        }
        else
        {
            rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, -25); //if knight is not ground, fall to ground
        }

        if(Vector2.Distance(PlayerMovement.Instance.transform.position, rigidbody2D.position) <= TheHollowKnight.Instance.GetAttackRange())
        {
            animator.SetBool("run", false);
        }
        else
        {
            return;
        }
    }
    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("run", false);
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
