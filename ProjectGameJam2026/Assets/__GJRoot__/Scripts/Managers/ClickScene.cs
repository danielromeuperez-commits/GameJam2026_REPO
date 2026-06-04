using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClickScene : MonoBehaviour
{
    [Header("Nombre de la escena a cargar")]
    [SerializeField] private string sceneName;

    [Header("Fade")]
    public CanvasGroup fadeCanvasGroup;
    public float fadeDuration = 1f;
    public float holdBlackTime = 0.25f;

    [Header("Music Fade")]
    public bool fadeMusic = true;
    public float musicFadeDuration = 1f;

    [Header("Fade Feel")]
    public bool usePulse = true;
    public float pulseScale = 1.05f;

    private bool isLoading;

    public void LoadScene()
    {
        if (isLoading) return;

        isLoading = true;
        StartCoroutine(LoadSceneWithFade());
    }

    private IEnumerator LoadSceneWithFade()
    {
        if (fadeCanvasGroup == null)
        {
            SceneManager.LoadScene(sceneName);
            yield break;
        }

        fadeCanvasGroup.gameObject.SetActive(true);
        fadeCanvasGroup.alpha = 0f;
        fadeCanvasGroup.blocksRaycasts = true;
        fadeCanvasGroup.interactable = true;

        RectTransform fadeRect = fadeCanvasGroup.GetComponent<RectTransform>();

        if (fadeRect != null)
            fadeRect.localScale = Vector3.one;

        if (fadeMusic)
            StartCoroutine(FadeOutMusic());

        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float t = timer / fadeDuration;

            float smoothT = Mathf.SmoothStep(0f, 1f, t);
            fadeCanvasGroup.alpha = smoothT;

            if (usePulse && fadeRect != null)
            {
                float pulse = Mathf.Sin(t * Mathf.PI) * (pulseScale - 1f);
                fadeRect.localScale = Vector3.one * (1f + pulse);
            }

            yield return null;
        }

        fadeCanvasGroup.alpha = 1f;

        if (fadeRect != null)
            fadeRect.localScale = Vector3.one;

        yield return new WaitForSeconds(holdBlackTime);

        SceneManager.LoadScene(sceneName);
    }

    private IEnumerator FadeOutMusic()
    {
        if (AudioManager.Instance == null) yield break;
        if (AudioManager.Instance.musicSource == null) yield break;

        AudioSource music = AudioManager.Instance.musicSource;

        float startVolume = music.volume;
        float timer = 0f;

        while (timer < musicFadeDuration)
        {
            timer += Time.deltaTime;
            float t = timer / musicFadeDuration;

            music.volume = Mathf.Lerp(startVolume, 0f, t);

            yield return null;
        }

        music.volume = 0f;
    }
}