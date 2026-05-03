using UnityEngine;

public class Ladder : MonoBehaviour
{
    private bool isInRange; // Indique si le joueur est à proximité de l'échelle
    private PlayerMovement playerMovement;
    public BoxCollider2D collider;

    void Awake()
    {
        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
    }
    void Update()
{
    // On vérifie d'abord si on veut SORTIR de l'échelle
    if (playerMovement.isClimbing && Input.GetKeyDown(KeyCode.E))
    {
        playerMovement.isClimbing = false;
        collider.isTrigger = false;
    }
    // Sinon, on vérifie si on veut MONTER
    else if (isInRange && Input.GetKeyDown(KeyCode.E))
    {
        playerMovement.isClimbing = true;
        collider.isTrigger = true;
    }
}
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isInRange = true; // Le joueur est à proximité de l'échelle
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                isInRange = false; // Le joueur n'est plus à proximité de l'échelle
                playerMovement.isClimbing = false; // Le joueur arrête de grimper
                collider.isTrigger = false; // Le joueur ne peut plus traverser les plateformes
            }
        }


}
