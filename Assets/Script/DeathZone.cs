using UnityEngine;

public class DeathZone : MonoBehaviour
{
    private Transform SpawnPlayer;

    private void Awake()
    {
        // Trouve la position du point de spawn
        SpawnPlayer = GameObject.FindGameObjectWithTag("SpawnPlayer").transform; 
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // 1. On récupère le script HealthManager sur le joueur
            HealthManager playerHealth = collision.GetComponent<HealthManager>();

            if (playerHealth != null)
            {
                // 2. On inflige 1 dégât (sans recul particulier)
                playerHealth.TakeDamage(1, Vector2.zero);
            }

            // 3. On replace le joueur au spawn
            collision.transform.position = SpawnPlayer.position;

            // Optionnel : On remet sa vitesse à zéro pour éviter qu'il garde 
            // l'inertie de sa chute après le respawn
            Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
            }
        }
    }
}