using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheHollowKnight : Enemy
{
    public static TheHollowKnight Instance;
    public float attackTimer;
    [HideInInspector] public bool facingRight;

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
    }
    protected override void Start()
    {
        base.Start();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        animator = GetComponentInChildren<Animator>();
        ChangeState(EnemyStates.THK_Stage1);
        alive = true;
    }
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
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
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

        if(health <= 0)
        {
            animator.SetTrigger("die");
            //Death(2f);
        }

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
    //Function event set to animation parrying
    public void Parrying()
    {
        parrying = true;
    }


    public float GetDamage(){ return damage;}
    public float GetRange() {  return range;}
    public float GetAttackRange() {  return range; }

    #region attacking
    #region variables
    [HideInInspector] public bool attacking;
    [HideInInspector] public float attackCountdown;
    [HideInInspector] public bool isDamagedPlayer = false;

    [HideInInspector] public bool parrying;

    [HideInInspector] public Vector2 moveToPosition;
    [HideInInspector] public bool diveAttack;
    public GameObject divingCollider;
    public GameObject pillar;


    #endregion //end variables

    #region Stage1
    IEnumerator TrippleSlash(){
        attacking = true;
        rigiBody2d.velocity = Vector2.zero;
        Debug.Log("set trigger attack");
        animator.SetTrigger("attack");
        yield return new WaitForSeconds(0.3f);
        animator.ResetTrigger("attack");

        animator.SetTrigger("attack");
        yield return new WaitForSeconds(0.5f);
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

        animator.SetBool("lunge", true);
        yield return new WaitForSeconds(1f);
        animator.SetBool("lunge", false);

        isDamagedPlayer = false;
        ResetAllAttacks();
    }

    IEnumerator Parry()
    {
        Flip();
        attacking = true;
        rigiBody2d.velocity = Vector2.zero;

        animator.SetBool("parry", true);
        yield return new WaitForSeconds(0.8f);
        animator.SetBool("lunge", false);

        ResetAllAttacks();
    }

    IEnumerator Slash()
    {
        Flip();
        yield return new WaitForSeconds(1f);
        attacking = true;
        rigiBody2d.velocity = Vector2.zero;

        animator.SetTrigger("attack");
        yield return new WaitForSeconds(0.2f);
        animator.ResetTrigger("Slash");

        ResetAllAttacks();

    }
    #endregion //end Stage1

    #region Stage2
    void DiveAttackJump()
    {
        attacking = true;
        moveToPosition = new Vector2 (PlayerMovement.Instance.transform.position.x, rigiBody2d.position.y + 10);
        diveAttack = true;
        animator.SetBool("jump", true);
    }

    public void Dive()
    {
        animator.SetBool("dive", true);
        animator.SetBool("jump", false);
    }

    private void OnTriggerEnter2D(Collider2D _other)
    {
        if(_other.GetComponent<PlayerAttack>() != null && diveAttack)
        {
            _other.GetComponent<PlayerAttack>().DamageEnemy();
        }
    }

    public void DivingPillars()
    {
        Vector2 impactPoint = groundCheckPoint.position;
        float _spawnDistance = 5;

        for (int i = 0; i < 10; i++)
        {
            Vector2 _pillarSpawnPointRight = impactPoint + new Vector2(_spawnDistance, 0);
            Vector2 _pillarSpawnPointLeft = impactPoint - new Vector2(_spawnDistance, 0);
            Instantiate(pillar, _pillarSpawnPointRight, Quaternion.Euler(0, 0, -90));
            Instantiate(pillar, _pillarSpawnPointLeft, Quaternion.Euler(0, 0, -90));
            _spawnDistance += 5;
        }
        ResetAllAttacks();
    }
        

    #endregion //end Stage2

    #region Control
    public void AttackHandler(){
        if(currentEnemyState == EnemyStates.THK_Stage1){
            if(rigiBody2d == null)
            {
                Debug.Log(" rigibody 2d = null");
                return;
            }
            if(Vector2.Distance(PlayerMovement.Instance.transform.position, rigiBody2d.position) <= GetAttackRange())
            {
                StartCoroutine(TrippleSlash());
            }
            else
            {
                StartCoroutine(Lunge());
            }
        }
    }
    public void ResetAllAttacks()
    {
        attacking = false;
        parrying = false;
        diveAttack = false;
        StopCoroutine(TrippleSlash());
        StopCoroutine(Lunge());
        StopCoroutine(Parry());
        StopCoroutine(Slash());
    }
    #endregion //end Control

    #endregion //end attacking

    public override void EnemyGetHit(float _damageDone, float _hitforce = 0)
    {
        if (!parrying)
        {
            base.EnemyGetHit(_damageDone, _hitforce);
            if (currentEnemyState != EnemyStates.THK_Stage4) {
                ResetAllAttacks(); //cancel any current attack to avoid bugs
                StartCoroutine(Parry());
            }
        }
        else
        {
            StopCoroutine(Parry());
            ResetAllAttacks();
            StartCoroutine(Slash()); //riposte
        }
    }
}
