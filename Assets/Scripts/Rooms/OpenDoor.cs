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
        HealthPlayer.OnEnemyDied += OnEnemyDied;
        
    }

    private void OnDisable() {
        //print("OnDisable opendoor");
        HealthPlayer.OnEnemyDied -= OnEnemyDied;
    }

    public void OnEnemyDied(HealthPlayer enemyHeal){
        foreach(GameObject e in enemies){
            //if(e.GetComponent<Health>() == enemyHeal){
            if(e.GetComponentInChildren<HealthPlayer>() == enemyHeal){
                aliveEnemyCount--;
                if(aliveEnemyCount <= 0)
                    gameObject.SetActive(false);
                break;
            }
        }
    }
}
