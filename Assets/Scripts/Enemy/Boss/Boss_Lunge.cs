using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Lunge : StateMachineBehaviour
{
    Rigidbody2D rigibody2D;
    HealthPlayer playerHealth;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        rigibody2D = animator.GetComponent<Rigidbody2D>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        rigibody2D.gravityScale = 0;
        int _dir = TheHollowKnight.Instance.facingRight ? 1: -1;
        rigibody2D.velocity = new Vector2(_dir * (TheHollowKnight.Instance.speed * 5), 0);
        if(Vector2.Distance(PlayerMovement.Instance.transform.position, rigibody2D.position) <= TheHollowKnight.Instance.GetAttackRange() 
            && !TheHollowKnight.Instance.isDamagedPlayer)
        {
            TheHollowKnight.Instance.DamagePlayer(TheHollowKnight.Instance.GetDamageLunge());
            TheHollowKnight.Instance.isDamagedPlayer = true;
            rigibody2D.gravityScale = 5;
        } 
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}
