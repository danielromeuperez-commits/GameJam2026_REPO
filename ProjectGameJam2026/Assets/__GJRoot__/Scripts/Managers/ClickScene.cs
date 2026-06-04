using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClickScene : MonoBehaviour
{
    [Header("Nombre de la escena a cargar")]
    [SerializeField] string sceneName;

    public void LoadScene()
    {
        SceneManager.LoadScene(sceneName);
    }
}