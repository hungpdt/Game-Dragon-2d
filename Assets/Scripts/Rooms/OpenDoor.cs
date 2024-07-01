using UnityEngine;

public class OpenDoor : MonoBehaviour
{

    [Header("Enemies")]
    [SerializeField] private GameObject[] enemies;
    private int aliveEnemyCount = 0;

    private void Start() {
        //print("Start opendoor");
        aliveEnemyCount = enemies.Length;
    }

    private void OnEnable() {
        //print("OnEnable opendoor");
        Health.OnEnemyDied += OnEnemyDied;
        
    }

    private void OnDisable() {
        //print("OnDisable opendoor");
        Health.OnEnemyDied -= OnEnemyDied;
    }

    public void OnEnemyDied(Health enemyHeal){
        foreach(GameObject e in enemies){
            //if(e.GetComponent<Health>() == enemyHeal){
            if(e.GetComponentInChildren<Health>() == enemyHeal){
                aliveEnemyCount--;
                if(aliveEnemyCount <= 0)
                    gameObject.SetActive(false);
                break;
            }
        }
    }
}
