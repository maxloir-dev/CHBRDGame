using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public int attackDamage = 2;
    public float attackRange = 1f;
    public LayerMask enemyLayer;

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Attack();
        }
    }

    void Attack()
    {
        Vector2 attackPosition = transform.position;

        Collider2D enemy = Physics2D.OverlapCircle(
            attackPosition,
            attackRange,
            enemyLayer
        );

    if (enemy != null)
{
    Debug.Log("ENNEMI DÉTECTÉ : " + enemy.name);

    EnemyHealth enemyHealth =
        enemy.transform.root.GetComponentInChildren<EnemyHealth>();

    if (enemyHealth != null)
    {
        Debug.Log(
    "JOUEUR X = " + transform.position.x +
    " | ENNEMI X = " + enemy.transform.position.x
);
     enemyHealth.TakeDamage(
    attackDamage,
    transform.position,
    enemy.transform.position
);
    }
    else
    {
        Debug.Log("L'ennemi n'a pas EnemyHealth !");
    }
}
else
{
    Debug.Log("Aucun ennemi détecté");
}
    } }
