using UnityEngine;
using UnityEngine.SceneManagement; // Obligatoire pour gérer les scènes

public class GameOverManager : MonoBehaviour
{
    public GameObject gameOverUI; // On y glissera ton Panel "GameOverWindow"

    public void OnGameOver()
    {
        gameOverUI.SetActive(true); // Affiche l'écran de mort
        Time.timeScale = 0f;        // Arrête le temps (le jeu se fige)
    }

    public void RetryButton()
    {
        Time.timeScale = 1f;        // Très important : on remet le temps à 1 avant de recharger !
        // Recharge la scène actuelle
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}