using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private Transform SpawnPlayer; // La position du point de spawn

    private void Awake()
    {
        // Trouve la position du point de spawn
        SpawnPlayer = GameObject.FindGameObjectWithTag("SpawnPlayer").transform;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // 1. On replace le point de spawn à la position de ce checkpoint
            SpawnPlayer.position = transform.position;
            gameObject.GetComponent<BoxCollider2D>().enabled = false; // Désactive le collider pour éviter de réactiver le checkpoint

        }
    }
}
