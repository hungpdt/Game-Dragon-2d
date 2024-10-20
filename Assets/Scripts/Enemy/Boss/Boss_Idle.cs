using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Idle : StateMachineBehaviour
{
    Rigidbody2D rigidbody2D;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        rigidbody2D = animator.GetComponent<Rigidbody2D>();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       rigidbody2D.velocity = Vector2.zero;
       RunToPlayer(animator);
       if(TheHollowKnight.Instance.attackCountdown <= 0)
       {
            TheHollowKnight.Instance.AttackHandler();
            TheHollowKnight.Instance.attackCountdown = Random.Range(TheHollowKnight.Instance.attackTimer - 1,
                TheHollowKnight.Instance.attackTimer + 1);
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
}
