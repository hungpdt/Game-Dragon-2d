using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Bounce2 : StateMachineBehaviour
{
    Rigidbody2D rigidbody2D;
    bool callOnce;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        rigidbody2D = animator.GetComponent<Rigidbody2D>();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Vector2 _forceDirection = new Vector2(Mathf.Cos(Mathf.Deg2Rad * TheHollowKnight.Instance.rotationDirectionToTarget),
            Mathf.Sin(Mathf.Deg2Rad * TheHollowKnight.Instance.rotationDirectionToTarget));
        rigidbody2D.AddForce(_forceDirection * 3, ForceMode2D.Impulse);

        TheHollowKnight.Instance.boundCollider.SetActive(true);

        if (TheHollowKnight.Instance.isGrounded())
        {
            TheHollowKnight.Instance.boundCollider.SetActive(false);
            if (!callOnce)
            {
                TheHollowKnight.Instance.ResetAllAttacks();
                TheHollowKnight.Instance.CheckBounce() ;
                callOnce = true;
            }
            animator.SetTrigger("grounded");
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger("grounded");
        animator.ResetTrigger("bounce2");
        callOnce = false;
    }
}
