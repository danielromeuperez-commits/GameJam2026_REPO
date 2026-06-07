using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneChange : MonoBehaviour
{
    public string nombrEscena;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(changeWhenTime());
        AudioManager.Instance.PlaySFX(8);
    }

private IEnumerator changeWhenTime()
    {
        yield return new WaitForSeconds(3.5f);
        SceneManager.LoadScene(nombrEscena);
    }
}
