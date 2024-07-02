using System.Linq;
using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField] private GameObject[] enemies;
    private Vector3[] initialPosition;
    private bool[] initialActiveStates;

    private void Awake()
    {
        // Save the initial positions and active states of the enemies
        initialPosition = new Vector3[enemies.Length];
        initialActiveStates = new bool[enemies.Length];
        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i] != null)
            {
                initialPosition[i] = enemies[i].transform.position;
                initialActiveStates[i] = enemies[i].activeSelf;
            }
        }

        // Deactivate rooms if it's not the first room
        if (transform.GetSiblingIndex() != 0)
            ActivateRoom(false);
    }

    public void ActivateRoom(bool _status)
    {
        // Activate/deactivate enemies
        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i] != null)
            {
                if (_status)
                {
                    enemies[i].SetActive(initialActiveStates[i]);
                    enemies[i].transform.position = initialPosition[i];
                }
                else
                {
                    enemies[i].SetActive(false);
                }
                Debug.Log("SetActive = " + _status + " enemy " + enemies[i].name);
            }
        }
    }
}
