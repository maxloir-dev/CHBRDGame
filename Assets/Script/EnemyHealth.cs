using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    public int health = 10;
    public Rigidbody2D rb;
    public float knockbackForce = 1f;
    public float knockbackTime = 0.15f;

  public void TakeDamage(int damage, Vector2 playerPosition, Vector2 enemyPosition)
{
    health -= damage;

    Vector2 direction;

    if (playerPosition.x < enemyPosition.x)
    {
        direction = Vector2.right;
    }
    else
    {
        direction = Vector2.left;
    }

    Debug.Log("DIRECTION FINALE = " + direction);

    rb.linearVelocity = direction * knockbackForce;

        EnnemyPatrol patrol = GetComponent<EnnemyPatrol>();

        if (patrol != null)
        {
            patrol.isKnockedBack = true;
            StartCoroutine(KnockbackDelay());
        }

        Debug.Log("Ennemi touché ! Vie restante : " + health);

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator KnockbackDelay()
{
    yield return new WaitForSeconds(knockbackTime);

    rb.linearVelocity = Vector2.zero;

    EnnemyPatrol patrol = GetComponent<EnnemyPatrol>();

    if (patrol != null)
    {
        patrol.isKnockedBack = false;
    }
}
}