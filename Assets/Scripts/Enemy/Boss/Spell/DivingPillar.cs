using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DivingPillar : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D _other)
    {
        if(_other.CompareTag("Player"))
        {
            _other.GetComponent<HealthPlayer>().TakeDamagePlayer(TheHollowKnight.Instance.GetDamageDivingPillar());
        }
    }
}
a