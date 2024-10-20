using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Bounce1 : StateMachineBehaviour
{
    Rigidbody2D rigidbody2D;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        rigidbody2D = animator.GetComponent<Rigidbody2D>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (TheHollowKnight.Instance.bounceAttack)
        {
            Vector2 _newPosition = Vector2.MoveTowards(rigidbody2D.position, TheHollowKnight.Instance.moveToPosition,
                TheHollowKnight.Instance.speed * Random.Range(2, 4) * Time.fixedDeltaTime);

            rigidbody2D.MovePosition(_newPosition);
            if(Vector2.Distance(rigidbody2D.position, _newPosition) < 0.1f)
            {
                TheHollowKnight.Instance.CalculateTargetAngle();
                animator.SetTrigger("bounce2");
            }
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger("bounce1");
    }
}
