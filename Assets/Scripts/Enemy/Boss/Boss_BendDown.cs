using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_BendDown : StateMachineBehaviour
{
    Rigidbody2D rigidbody2D;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       rigidbody2D = animator.GetComponent<Rigidbody2D>();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        OutbreakAttack();
    }

    void OutbreakAttack()
    {
        if (TheHollowKnight.Instance.outbreakAttack)
        {
            Vector2 _newPosition = Vector2.MoveTowards(rigidbody2D.position, TheHollowKnight.Instance.moveToPosition,
                TheHollowKnight.Instance.speed * 1.5f * Time.fixedDeltaTime);
            rigidbody2D.MovePosition(_newPosition);

            if(Vector2.Distance(rigidbody2D.position, _newPosition)  < 0.1f)
            {
                TheHollowKnight.Instance.rigiBody2d.constraints = RigidbodyConstraints2D.FreezeRotation;
                TheHollowKnight.Instance.rigiBody2d.gravityScale = 0;
            }
        }
    }
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger("bendDown");
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
