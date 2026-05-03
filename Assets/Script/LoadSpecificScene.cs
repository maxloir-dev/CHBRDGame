using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoadSpecificScene : MonoBehaviour

{
    public string sceneName; // Le nom de la scène à charger, à définir dans l'inspecteur
    public Animator fadeSystem;

private void Awake()
    {
        fadeSystem = GameObject.FindGameObjectWithTag("FadeSystem").GetComponent<Animator>(); // Trouve l'Animator du système de fondu
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(LoadNextScene()); // Démarre la coroutine pour charger la scène
        }
    }
    public IEnumerator LoadNextScene()
    {
        fadeSystem.SetTrigger("FadeIn"); // Joue l'animation de fondu
        yield return new WaitForSeconds(1f); // Attends que l'animation de fondu soit terminée (ajustez la durée selon votre animation)
        SceneManager.LoadScene(sceneName); // Charge la scène spécifiée
    }
}
