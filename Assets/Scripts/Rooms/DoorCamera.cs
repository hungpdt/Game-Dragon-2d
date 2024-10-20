using UnityEngine;

public class DoorCamera : MonoBehaviour
{
    [SerializeField] private Transform previousRoom;
    [SerializeField] private Transform nextRoom;
    [SerializeField] private CameraController cam;
    private Vector3 lastPlayerPosition;
    private bool isPlayerInTrigger = false;

    private void Awake()
    {
        cam = Camera.main.GetComponent<CameraController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            lastPlayerPosition = collision.transform.position;
            isPlayerInTrigger = true;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && isPlayerInTrigger)
        {
            Vector3 currentPlayerPosition = collision.transform.position;

            // only switch camera when player actually moves go through the door
            if (currentPlayerPosition.x > transform.position.x && lastPlayerPosition.x <= transform.position.x)
            {
                cam.MoveToNewRoom(nextRoom);
                nextRoom.GetComponent<Room>().ActivateRoom(true);
                previousRoom.GetComponent<Room>().ActivateRoom(false);
            }
            else if (currentPlayerPosition.x < transform.position.x && lastPlayerPosition.x >= transform.position.x)
            {
                cam.MoveToNewRoom(previousRoom);
                previousRoom.GetComponent<Room>().ActivateRoom(true);
                nextRoom.GetComponent<Room>().ActivateRoom(false);
            }

            lastPlayerPosition = currentPlayerPosition;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInTrigger = false;
        }
    }
}
