using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected float health;
    // [SerializeField] float recoilLength;
    // [SerializeField] float recoilFactor;
    // [SerializeField] bool isRecolling = false;

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
    Rigidbody2D rb;
    public virtual void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    public virtual void Update() {
        if(health <= 0){
            Destroy(gameObject);
        }
    }

    public void EnemyHit(float _damageDone, Vector2 _hitDirection){
        health -= _damageDone;
    }


}
