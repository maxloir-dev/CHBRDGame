using UnityEngine;

public class WeakSpot : MonoBehaviour
{
    public GameObject objectToDestroy; // L'objet à détruire lorsque le point faible est touché
private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Destroy(objectToDestroy); // On détruit l'objet spécifié
        }
    }
}
