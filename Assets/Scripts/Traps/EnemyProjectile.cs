using UnityEngine;

public class EnemyProjectile : EnemyDamage
{
    [SerializeField] private float speed;
    [SerializeField] private float resetTime;
    private float lifetime;
    private Animator anim;
    private BoxCollider2D coll;

    private bool hit;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        coll = GetComponent<BoxCollider2D>();
    }

    public void ActivateProjectile()
    {
        hit = false;
        lifetime = 0;
        gameObject.SetActive(true);
        coll.enabled = true;
    }
    private void Update()
    {
        if (hit) return;
        float movementSpeed = speed * Time.deltaTime;
        transform.Translate(movementSpeed, 0, 0);

        lifetime += Time.deltaTime;
        if (lifetime > resetTime){
            gameObject.SetActive(false);
            Debug.Log("projectile ", gameObject );
        }
           
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //print("[EnemyProjecttile] OnTriggerEnter2D ");
        hit = true;
        base.OnTriggerEnter2D(collision); //Execute logic from parent script first
        coll.enabled = false;

        if (anim != null){
            //print("[EnemyProjecttile] trigger ani 'explode' ");
            anim.SetTrigger("explode"); //When the object is a fireball explode it
        }else{
            //print("[EnemyProjecttile] SetActive false ");
            gameObject.SetActive(false); //When this hits any object deactivate arrow
        }
    }
    private void Deactivate()
    {
        Debug.Log("********** setactive false gameobject" + gameObject.name );
        gameObject.SetActive(false);
    }
}