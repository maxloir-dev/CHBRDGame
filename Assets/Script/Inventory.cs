using UnityEngine;
using TMPro; 

public class Inventory : MonoBehaviour
{
    public int coinsCount;
    public TextMeshProUGUI coinsCountText; 

    public static Inventory instance;

    private void Awake()
    {
        if(instance != null)
        {
            Debug.LogWarning("Il y a plus d'une instance de Inventory dans la scène. Destruction du doublon.");
            Destroy(gameObject); // Très important : on détruit la copie pour ne pas avoir de bugs
            return;
        }

        instance = this;
        // Indispensable pour que les pièces restent d'une zone à l'autre !
        DontDestroyOnLoad(gameObject); 
    }

    public void AddCoins(int count)
    {
        coinsCount += count;
        // On vérifie que le texte existe bien avant d'écrire dedans pour éviter les erreurs
        if(coinsCountText != null)
        {
            coinsCountText.text = coinsCount.ToString(); 
        }
    }
}