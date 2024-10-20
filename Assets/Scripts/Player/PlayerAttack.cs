using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("RangeAttack")]
    [SerializeField] private float attackCooldown;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject[] fireballs;
    [SerializeField] private AudioClip fireballSound;

    private Animator anim;
    private PlayerMovement playerMovement;
    //private EnemyGetHit enemyHealth;
    
    private bool isAttack;
    private float cooldownTimer = Mathf.Infinity;
    [Space(5)]


    [Header("MeleeAttack")]
    private bool meleeAttack;
    [SerializeField] private float timeSinceAttack;
    [SerializeField] private float timeBetweenAttack;
    [SerializeField] private int damage;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
    }
    [Space(5)]

    [Header("Collider Parameters")]
    [SerializeField] private float colliderDistance;
    
    [SerializeField] private float range;
    [SerializeField] private BoxCollider2D boxCollider;
    [Space(5)]

    [Header("Enemy Layer")]
    [SerializeField] private LayerMask enemyLayer;
    private void Update()
    {
        if ( (isAttack || Input.GetKeyDown(KeyCode.Space)) && cooldownTimer > attackCooldown 
            && playerMovement.canAttack()&& Time.timeScale > 0){
            //RangeAttack();
            MeleeAttackButton();
            MeleeAttack();
            isAttack = false;
        }
        cooldownTimer += Time.deltaTime;

        //if(isAttack){
        //    //Debug.Log("melee attack");
        //    MeleeAttackButton();
        //    MeleeAttack();
        //}
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(boxCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance,
            new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y, boxCollider.bounds.size.z));
    }

    private bool EnemyInSight()
    {
        RaycastHit2D hit =
            Physics2D.BoxCast(boxCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance,
            new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y, boxCollider.bounds.size.z),
            0, Vector2.left, 0, enemyLayer);

        return hit.collider != null;
    }

    public void DamageEnemy(){
        if(EnemyInSight()){
            TheHollowKnight.Instance.EnemyGetHit(damage);
        }
    }
    public void AttackButton(){
        isAttack = true;
        //print("isAttack = true");
    }
    public void MeleeAttackButton(){
        meleeAttack = true;
        //print("isAttack = true");
    }
    private void MeleeAttack(){
        timeSinceAttack += Time.deltaTime;

        if(meleeAttack && timeSinceAttack >= timeBetweenAttack){
            //Debug.Log("set trigger melee attack");
            anim.SetTrigger("melee_attack");
            timeSinceAttack = 0;
        }
    }
    private void RangeAttack()
    {
        SoundManager.instance.PlaySound(fireballSound);
        anim.SetTrigger("attack");
        cooldownTimer = 0;

        fireballs[FindFireball()].transform.position = firePoint.position;
        fireballs[FindFireball()].GetComponent<Projectile>().SetDirection(Mathf.Sign(transform.localScale.x));
    }
    private int FindFireball()
    {
        for (int i = 0; i < fireballs.Length; i++)
        {
            if (!fireballs[i].activeInHierarchy)
                return i;
        }
        return 0;
    }
}