using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrageFireBall : MonoBehaviour
{
    [SerializeField] Vector2 startForceMinMax;
    [SerializeField] float turnSpeed = 0.5f;

    Rigidbody2D rigibody2D;
    // Start is called before the first frame update
    void Start()
    {
        rigibody2D = GetComponent<Rigidbody2D>();
        Destroy(gameObject, 4f);
        rigibody2D.AddForce(transform.right * Random.Range(startForceMinMax.x, startForceMinMax.y), ForceMode2D.Impulse);
    }

    // Update is called once per frame
    void Update()
    {
        float maxSpeed = 15f;
        if (rigibody2D.velocity.magnitude > maxSpeed)
        {
            rigibody2D.velocity = rigibody2D.velocity.normalized * maxSpeed;
        }

        var _dir = rigibody2D.velocity;
        if(_dir != Vector2.zero)
        {
            Vector3 _frontVector = Vector3.right;

            Quaternion _targetRotation = Quaternion.FromToRotation(_frontVector, _dir - (Vector2)transform.position);
            if(_dir.x > 0)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, _targetRotation, turnSpeed);
                transform.eulerAngles = new Vector3(transform.eulerAngles.x, 180, transform.eulerAngles.z);
            }
            else
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, _targetRotation, turnSpeed);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            collision.GetComponent<HealthPlayer>().TakeDamagePlayer(TheHollowKnight.Instance.GetDamage());
            Destroy(gameObject);
        }
    }
}
