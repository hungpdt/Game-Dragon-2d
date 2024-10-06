using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_jump : StateMachineBehaviour
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
        DiveAttack();
    }

    void DiveAttack()
    {
        if (TheHollowKnight.Instance.diveAttack)
        {
            TheHollowKnight.Instance.Flip();
            Vector2 _newPos = Vector2.MoveTowards(rigidbody2D.position, TheHollowKnight.Instance.moveToPosition,
                TheHollowKnight.Instance.speed * 3 * Time.fixedDeltaTime);

            rigidbody2D.MovePosition(_newPos);

            float _distance = Vector2.Distance(rigidbody2D.position, _newPos);
            if(_distance < 0.1)
            {
                TheHollowKnight.Instance.Dive();
            }
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
