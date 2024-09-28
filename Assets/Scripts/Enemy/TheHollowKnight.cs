using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheHollowKnight : Enemy
{
    public static TheHollowKnight Instance;
    [SerializeField] GameObject slashEffect;

    public float attackRange;
    public float attackTimer;
    [HideInInspector] public bool facingRight;

    [Header("Ground Check settings")]
    //[SerializeField] private Transform groundCheckPoint;
    //[SerializeField] private float groundCheckY = 0.2f;
    //[SerializeField] private float groundCheckX = 0.5f;
    //[SerializeField] private LayerMask groundLayer;

    int hitCounter;
    bool stunned, canStun;
    bool alive;

    [HideInInspector] public float runSpeed;
    protected override void Awake() {
        base.Awake();
        if(Instance != null && Instance != this){
            Destroy(gameObject);
        } else{
            Instance = this;
        }

        boxCollider2D = GetComponent<BoxCollider2D>();
    }
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        animator = GetComponentInChildren<Animator>();
        ChangeState(EnemyStates.THK_Stage1);
        alive = true;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (!attacking)
        {
            attackCountdown -= Time.deltaTime;
        }
    }

    public bool isGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider2D.bounds.center, boxCollider2D.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
        if (raycastHit.collider != null)
        {
            //Debug.Log(" player is in grounded");
            return true;
        }
        else
        {
            //Debug.Log("player is NOT in grounded");
            return false;
        }
    }
    public void Flip(){
        if(PlayerMovement.Instance.transform.position.x > transform.position.x){
            transform.localScale = Vector3.one;
            facingRight = true;
        } else {
            transform.localScale = new Vector3(-1, 1, 1);
            facingRight = false;
        }
    }
    protected override void UpdateEnemyStates(){
        if(PlayerMovement.Instance != null){
            switch (GetCurrentEnemyState())
            {
                case EnemyStates.THK_Stage1:
                    break;

                case EnemyStates.THK_Stage2:
                    break;

                case EnemyStates.THK_Stage3:
                    break;

                case EnemyStates.THK_Stage4:
                    break;
            }
        }
    }

    #region attacking
    #region variables
    [HideInInspector] public bool attacking;
    [HideInInspector] public float attackCountdown;
    #endregion //end variables

    #region Stage1
    IEnumerator TrippleSlash(){
        attacking = true;
        rigiBody2d.velocity = Vector2.zero;
        Debug.Log("set trigger attack");
        animator.SetTrigger("attack");
        yield return new WaitForSeconds(1f);
        animator.ResetTrigger("attack");

        animator.SetTrigger("attack");
        yield return new WaitForSeconds(1.5f);
        animator.ResetTrigger("attack");

        animator.SetTrigger("attack");
        yield return new WaitForSeconds(0.8f);
        animator.ResetTrigger("attack");

        ResetAllAttacks();
    }

    IEnumerator Lunge()
    {
        Flip();
        attacking = true;

        animator.SetBool("Lungle", true);
        yield return new WaitForSeconds(1f);
        animator.SetBool("Lungle", false);

        ResetAllAttacks();
    }
    #endregion //end Stage1

    #region Control
    public void AttackHandler(){
        if(currentEnemyState == EnemyStates.THK_Stage1){
            if(rigiBody2d == null)
            {
                Debug.Log(" rigibody 2d = null");
                return;
            }
            if(Vector2.Distance(PlayerMovement.Instance.transform.position, rigiBody2d.position) <= attackRange){
                StartCoroutine(TrippleSlash());
            }else{
                return;
            }
        }
    }
    public void ResetAllAttacks()
    {
        attacking = false;
        StopCoroutine(TrippleSlash());
    }
    #endregion //end Control
    
    #endregion //end attacking
}
