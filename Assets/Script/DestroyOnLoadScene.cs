using UnityEngine;

public class DestroyOnLoadScene : MonoBehaviour
{
   public GameObject[] objects;

   void Awake()
   {
       foreach (var element in objects)
       {
           DontDestroyOnLoad(element); // Empêche l'objet de se détruire lors du chargement d'une nouvelle scène
       }
   }

}
