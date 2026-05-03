using UnityEngine;

public class SpawnPlayer : MonoBehaviour
{
    private void Awake()

    {
       GameObject.FindGameObjectWithTag("Player").transform.position = transform.position; // Place le joueur à la position de ce GameObject
    }
}
