using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections; // Nécessaire pour le clignotement

public class HealthManager : MonoBehaviour
{
    public int health;
    public int maxHealth;

    public List<Image> hearts;
    public Sprite fullHeart;

    public GameOverManager gameOverManager;

    public Rigidbody2D rb; 
    public float knockbackForce = 5f;

    public float invincibilityTime = 1.5f; // Temps d'invincibilité en secondes
    private bool isInvincible = false;

    public SpriteRenderer playerGraphics; // Pour faire clignoter le perso

  void Update()
{
    if (health > maxHealth) health = maxHealth;

    for (int i = 0; i < hearts.Count; i++)
    {
        // On vérifie si l'index du cœur est inférieur à la vie actuelle
        if (i < health) 
        {
            hearts[i].gameObject.SetActive(true); // Affiche l'objet complet
        }
        else 
        {
            hearts[i].gameObject.SetActive(false); // Cache l'objet complet
        }
    }
}

public void TakeDamage(int damage, Vector2 damageDirection)
{
    if (isInvincible) return;

    health -= damage;

    // --- NOUVEAU : AJOUT DU RECUL ---
    // On propulse le joueur dans la direction opposée au choc
    rb.linearVelocity = Vector2.zero; // On remet la vitesse à 0 pour que le choc soit net
    rb.AddForce(damageDirection * knockbackForce, ForceMode2D.Impulse);

    if (health > 0)
    {
        StartCoroutine(InvincibilityDelay());
    }
    else
    {
        health = 0;
    gameOverManager.OnGameOver();
    }
}

    // Le "timer" d'invincibilité qui fait aussi clignoter
   private IEnumerator InvincibilityDelay()
{
    isInvincible = true;

    // DESACTIVE la collision entre le layer Player (6) et Ennemi (7)
    Physics2D.IgnoreLayerCollision(6, 7, true);
    
    for (float i = 0; i < invincibilityTime; i += 0.2f)
    {
        playerGraphics.color = new Color(1, 1, 1, 0.2f);
        yield return new WaitForSeconds(0.1f);
        playerGraphics.color = new Color(1, 1, 1, 1f);
        yield return new WaitForSeconds(0.1f);
    }

    // RE-ACTIVE la collision après le clignotement
    Physics2D.IgnoreLayerCollision(6, 7, false);

    isInvincible = false;
}
}