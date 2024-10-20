using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheHollowKnight : Enemy
{
    public static TheHollowKnight Instance;
    public float attackTimer;
    [HideInInspector] public bool facingRight;

    [Header("Damage")]
    [SerializeField] private float damageDivingPillar;
    [SerializeField] private float damageLunge;
    [Space(5)]

    [Header("Stun")]
    [SerializeField] private int hitToStun;
    int hitCounter;
    bool stunned, canStun;
    [Space(5)]

    bool alive;

    [HideInInspector] public float runSpeed;
    protected override void Awake() {
        base.Awake();
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
        } else {
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

        if(health <= 0 && alive)
        {
            Death(0);
        }
        if (!attacking)
        {
            attackCountdown -= Time.deltaTime;
        }

        if (stunned)
        {
            rigiBody2d.velocity = Vector2.zero;
        }
    }

    public bool isGrounded()
    {
        if (Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, -transform.up, 0.12f, groundLayer))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public void Flip() {
        if (PlayerMovement.Instance.transform.position.x > transform.position.x) {
            transform.localScale = Vector3.one;
            facingRight = true;
        } else {
            transform.localScale = new Vector3(-1, 1, 1);
            facingRight = false;
        }
    }
    protected override void UpdateEnemyStates() {

        if (health <= 0)
        {
            animator.SetTrigger("die");
            Death(2f);
        }

        if (PlayerMovement.Instance != null) {
            switch (GetCurrentEnemyState())
            {
                case EnemyStates.THK_Stage1:
                    canStun = true;
                    attackTimer = 6;
                    runSpeed = speed;
                    break;

                case EnemyStates.THK_Stage2:
                    canStun = true;
                    attackTimer = 5;
                    break;

                case EnemyStates.THK_Stage3:
                    canStun = false;
                    attackTimer = 8;
                    break;

                case EnemyStates.THK_Stage4:
                    canStun = false;
                    attackTimer = 10;
                    runSpeed = speed/2;
                    break;
            }
        }
    }

    //The Hollow Knight Event
    #region The Hollow Knight Event
    public void Parrying() //Function event set to animation parrying
    {
        parrying = true;
    }

    public void BendDownCheck()
    {
        if (barrageAttack)
        {
            StartCoroutine(BarrageAttackTransition());
        }

        if (outbreakAttack)
        {
            StartCoroutine(OutbreakAttackTrasition());
        }

        if (bounceAttack)
        {
            animator.SetTrigger("bounce1");
        }
    }
    IEnumerator BarrageAttackTransition()
    {
        yield return new WaitForSeconds(1f);
        animator.SetBool("cast", true);
    }
    void BarrageOrOutBreak()
    {
        if (barrageAttack)
        {
            StartCoroutine(Barrage());
        }

        if (outbreakAttack)
        {
            StartCoroutine(OutBreak());
        }
    }

    IEnumerator OutbreakAttackTrasition()
    {
        yield return new WaitForSeconds(1f);
        TheHollowKnight.Instance.animator.SetBool("cast", true);
    }

    #endregion //The Hollow Knight Event
    public float GetDamage() { return damage; }
    public float GetDamageDivingPillar() { return damageDivingPillar; }
    public float GetDamageLunge() { return damageLunge; }
    public float GetRange() { return range; }
    public float GetAttackRange() { return range; }

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

    [HideInInspector] public bool barrageAttack;
    public GameObject barrageFireball;
    [HideInInspector] public bool outbreakAttack;

    //bounce attack
    [HideInInspector] public bool bounceAttack;
    [HideInInspector] public float rotationDirectionToTarget;
    [HideInInspector] public int bounceCount;
    [SerializeField] public GameObject boundCollider;
    //Lunge
    [SerializeField] private float lungeTime;

    #endregion //end variables

    #region Stage1
    IEnumerator TrippleSlash(){
        attacking = true;
        rigiBody2d.velocity = Vector2.zero;
        //Debug.Log("set trigger attack");
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
        yield return new WaitForSeconds(lungeTime);
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
        yield return new WaitForSeconds(1f);
        animator.SetBool("lunge", false);
        animator.SetBool("parry", false);
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
        animator.ResetTrigger("attack");

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
        if(_other.GetComponent<HealthPlayer>() != null && (diveAttack || bounceAttack))
        {
            _other.GetComponent<HealthPlayer>().TakeDamagePlayer(TheHollowKnight.Instance.GetDamage());
        }
    }

    public void DivingPillars()
    {
        Vector2 impactPoint = groundCheckPoint.position;
        //Vector2 impactPoint = moveToPosition;
        float _spawnDistance = 5;
        for (int i = 0; i < 10; i++)
        {
            Vector2 _pillarSpawnPointRight = impactPoint + new Vector2(_spawnDistance, 0);
            Vector2 _pillarSpawnPointLeft = impactPoint - new Vector2(_spawnDistance, 0);
            Instantiate(pillar, _pillarSpawnPointRight, Quaternion.Euler(0, 0, -90));
            Instantiate(pillar, _pillarSpawnPointLeft, Quaternion.Euler(0, 0, -90));
            Debug.Log("Pillar Right Spawn Position: " + _pillarSpawnPointRight);
            Debug.Log("Pillar Left Spawn Position: " + _pillarSpawnPointLeft);

            _spawnDistance += 5;
        }
        ResetAllAttacks();
    }
    
    void BarrageBendDown()
    {
        attacking = true;
        rigiBody2d.velocity = Vector2.zero;
        barrageAttack = true;
        animator.SetTrigger("bendDown");
    }

    public IEnumerator Barrage()
    {
        Flip();
        rigiBody2d.velocity = Vector2.zero;
        float currentAngle = 30f;
        for(int i = 0; i < 20; i++)
        {
            GameObject _projectile = Instantiate(barrageFireball, transform.position, Quaternion.Euler(0, 0, currentAngle));
            if (facingRight)
            {
                _projectile.transform.eulerAngles = new Vector3(_projectile.transform.eulerAngles.x, 0, currentAngle);
            }
            else
            {
                _projectile.transform.eulerAngles = new Vector3(_projectile.transform.eulerAngles.x, 180, currentAngle);
            }
            currentAngle += 5f;
            yield return new WaitForSeconds(0.3f);
        }
        yield return new WaitForSeconds(0.1f);
        animator.SetBool("cast", false);
        ResetAllAttacks();
    }

    #endregion //end Stage2

    #region Stage3
    void OutBreakBendDown()
    {
        attacking = true;
        rigiBody2d.velocity = Vector2.zero;
        moveToPosition = new Vector2(transform.position.x, rigiBody2d.position.y + 5);
        outbreakAttack = true;
        animator.SetTrigger("bendDown");
    }

    public IEnumerator OutBreak()
    {
        Flip();
        yield return new WaitForSeconds(1f);
        animator.SetBool("cast", true);
        rigiBody2d.velocity = Vector2.zero;

        for(int i = 0; i < 30; i++)
        {
            Instantiate(barrageFireball, transform.position, Quaternion.Euler(0, 0, Random.Range(110, 130))); //downwards
            Instantiate(barrageFireball, transform.position, Quaternion.Euler(0, 0, Random.Range(50, 70))); //diagonally right
            Instantiate(barrageFireball, transform.position, Quaternion.Euler(0, 0, Random.Range(260, 280))); //diagonally left
            yield return new WaitForSeconds(0.2f);
        }
        yield return new WaitForSeconds(0.1f);

        rigiBody2d.constraints = RigidbodyConstraints2D.None;
        rigiBody2d.constraints = RigidbodyConstraints2D.FreezeRotation;

        rigiBody2d.velocity = new Vector2(rigiBody2d.velocity.x, -10);
        yield return new WaitForSeconds(0.1f);
        animator.SetBool("cast", false);
        rigiBody2d.gravityScale = 5;
        ResetAllAttacks();
    }

    void BounceAttack()
    {
        attacking = true;
        bounceCount = Random.Range(2, 5);
        BounceBendDown();
    }

    int _bounces = 0;
    public void CheckBounce()
    {
        if(_bounces < bounceCount - 1)
        {
            _bounces++;
            BounceBendDown();
        }
        else
        {
            _bounces = 0;
            animator.Play("Boss_walk");
        }
    }
    public void BounceBendDown()
    {
        rigiBody2d.velocity = Vector2.zero;
        moveToPosition = new Vector2(PlayerMovement.Instance.transform.position.x, rigiBody2d.position.y + 5);
        bounceAttack = true;
        animator.SetTrigger("bendDown");
    }
    public void CalculateTargetAngle()
    {
        Vector3 _directionToTarget = (PlayerMovement.Instance.transform.position - transform.position).normalized;

        float _angleOfTarget = Mathf.Atan2(_directionToTarget.y, _directionToTarget.x) * Mathf.Rad2Deg;
        rotationDirectionToTarget = _angleOfTarget;
    }
    #endregion//stage3
    #region Control
    public void AttackHandler(){
        switch (currentEnemyState)
        {
            case EnemyStates.THK_Stage1:
                if (IsPlayerInAtttackRange()){
                    StartCoroutine(TrippleSlash());
                }
                else
                {
                    //StartCoroutine(Lunge());
                    DiveAttackJump();
                }
                break;

            case EnemyStates.THK_Stage2:
                if (IsPlayerInAtttackRange())
                {
                    StartCoroutine(TrippleSlash());
                }
                else
                {
                    int _randomAttack = Random.Range(1, 3);
                    if (_randomAttack == 1)
                    {
                        StartCoroutine(Lunge());
                    }
                    else if (_randomAttack == 2)
                    {
                        DiveAttackJump();
                    }
                    else if(_randomAttack == 3)
                    {
                        BarrageBendDown();
                    }
                    StartCoroutine(Lunge());
                }
                break;

            case EnemyStates.THK_Stage3:
                int attackChosen = Random.Range(1, 4);
                if (attackChosen == 1)
                {
                    OutBreakBendDown();
                }
                else if (attackChosen == 2)

                {
                    DiveAttackJump();
                }
                else if(attackChosen == 3)
                {
                    BarrageBendDown();
                }
                else
                {
                    BounceAttack();
                }
                break;

            case EnemyStates.THK_Stage4:
                if (IsPlayerInAtttackRange())
                {
                    StartCoroutine(Slash());
                }
                else
                {
                    BounceAttack();
                }
                break;

            default:
                Debug.LogError("Don't have this state");
                break;
        }
    }
    public bool IsPlayerInAtttackRange()
    {
        return Vector2.Distance(PlayerMovement.Instance.transform.position, rigiBody2d.position) <= GetAttackRange();
    }
    public void ResetAllAttacks()
    {
        attacking = false;
        parrying = false;
        diveAttack = false;
        barrageAttack = false;
        outbreakAttack = false;
        bounceAttack = false;
        StopCoroutine(TrippleSlash());
        StopCoroutine(Lunge());
        StopCoroutine(Parry());
        StopCoroutine(Slash());
    }
    #endregion //end Control

    #endregion //end attacking

    public override void EnemyGetHit(float _damageDone, float _hitforce = 0)
    {
        if (!stunned)
        {
            if (!parrying)
            {
                if (canStun)
                {
                    hitCounter++;
                    Debug.Log("hitCounter = "+ hitCounter);
                    if(hitCounter >= hitToStun)
                    {
                        ResetAllAttacks();
                        StartCoroutine(Stunned());
                    }
                }
                base.EnemyGetHit(_damageDone, _hitforce);
                if (currentEnemyState != EnemyStates.THK_Stage4)
                {
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
        else
        {
            StopCoroutine(Stunned());
            animator.SetBool("stunned", false);
            stunned = false;
        }

        #region health to state
        if(health > 20)
        {
            ChangeState(EnemyStates.THK_Stage1);
        }
        if(health <= 15 && health > 10)
        {
            ChangeState(EnemyStates.THK_Stage2);
        }
        if (health <= 10 && health > 5)
        {
            ChangeState(EnemyStates.THK_Stage3);
        }
        if (health <= 5)
        {
            ChangeState(EnemyStates.THK_Stage4);
        }
        if(health <= 0)
        {
            Death(0);
        }
        #endregion //health to state
    }

    public IEnumerator Stunned()
    {
        stunned = true;
        hitCounter = 0;
        animator.SetBool("stunned", true);

        yield return new WaitForSeconds(6f);
        animator.SetBool("stunned", false);
        stunned = false;
    }

    protected override void Death(float _destroyTime)
    {
        ResetAllAttacks();
        alive = false;
        rigiBody2d.velocity = new Vector2(rigiBody2d.velocity.x, -10);
        animator.SetTrigger("die");
    }
    public void DestroyAfterDeath()
    {
        Destroy(gameObject);
    }
}
