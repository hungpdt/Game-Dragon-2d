using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    [SerializeField] protected float damage;

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        //print("[EnemyDamage] OnTriggerEnter2D ");
        if (collision.CompareTag("Player"))
            collision.GetComponent<HealthPlayer>()?.TakeDamagePlayer(damage);
    }
}