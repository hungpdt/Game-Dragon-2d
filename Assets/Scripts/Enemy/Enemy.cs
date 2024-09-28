using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected float health;
    [SerializeField] public float speed;
    protected SpriteRenderer spriteRenderer;
    protected Rigidbody2D rigiBody2d;
    protected Animator animator;
    // [SerializeField] float recoilLength;
    // [SerializeField] float recoilFactor;
    // [SerializeField] bool isRecolling = false;

    [Header("Ground Check settings")]
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private float groundCheckY = 0.2f;
    [SerializeField] private float groundCheckX = 0.5f;
    [SerializeField] public LayerMask groundLayer;
    protected BoxCollider2D boxCollider2D;
    protected EnemyStates currentEnemyState;

    protected enum EnemyStates{
        //Shade
        Shade_Idle,
        Shade_Chase,
        Shade_Stunned,
        Shade_Death, 

        //THK
        THK_Stage1,
        THK_Stage2,
        THK_Stage3,
        THK_Stage4
    }
    protected virtual void Awake() {
        rigiBody2d = GetComponent<Rigidbody2D>();
        //boxCollider2D = GetComponent<BoxCollider2D>();
    }

    protected virtual void Start(){

    }
    protected virtual void Update() {
        if(health <= 0){
            Destroy(gameObject);
        }
    }

    public void EnemyHit(float _damageDone, Vector2 _hitDirection){
        health -= _damageDone;
    }

    protected void ChangeState(EnemyStates _newState){
        currentEnemyState = _newState;
    }

    protected virtual void UpdateEnemyStates(){

    }

    protected EnemyStates GetCurrentEnemyState()
    {
        return currentEnemyState;
    }
    //public bool isGrounded()
    //{
    //    RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider2D.bounds.center, boxCollider2D.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
    //    if(raycastHit.collider != null){
    //        //Debug.Log(" player is in grounded");
    //        return true;
    //    } else{
    //        //Debug.Log("player is NOT in grounded");
    //        return false;
    //    }
    //}

    private bool PlayerInSight()
    {
        //RaycastHit2D hit =
        //    Physics2D.BoxCast(boxCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance,
        //    new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y, boxCollider.bounds.size.z),
        //    0, Vector2.left, 0, playerLayer);

        //if (hit.collider != null)
        //    playerHealth = hit.transform.GetComponent<Health>();

        //return hit.collider != null;
    }
}
