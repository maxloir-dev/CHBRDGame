using UnityEngine;

public class EnnemyPatrol : MonoBehaviour
{
    public float speed;
    public Transform[] waypoints;
    public SpriteRenderer graphics;

    public Transform target;
    private int desPoint = 0;

    // private bool movingRight = true;
    

void Start()
{
    target = waypoints[0];
}

void Update()
{
 Vector3 dir = target.position - transform.position;
 transform.Translate(dir.normalized * speed * Time.deltaTime, Space.World);

 if (Vector3.Distance(transform.position, target.position) < 0.3f)
    {
        desPoint = (desPoint + 1) % waypoints.Length;
        target = waypoints[desPoint];
        graphics.flipX = !graphics.flipX; // Inverse le sprite pour faire face à la direction opposée
    }
}
private void OnCollisionEnter2D(Collision2D collision)
{
    if (collision.gameObject.CompareTag("Player"))
    {
        HealthManager playerHealth = collision.gameObject.GetComponent<HealthManager>();
        if (playerHealth != null)
        {
            // On calcule la direction du recul (du monstre vers le joueur)
            Vector2 direction = (collision.transform.position - transform.position).normalized;
            // On inflige 1 dégât
            playerHealth.TakeDamage(1, direction);
        }
    }
}
}