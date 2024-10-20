using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorScene : MonoBehaviour
{
    [SerializeField] private int nextScence;
    private void OnTriggerEnter2D(Collider2D collider) {
        if(collider.CompareTag("Player")){
            SceneManager.LoadScene(nextScence);
        }
    }
}
