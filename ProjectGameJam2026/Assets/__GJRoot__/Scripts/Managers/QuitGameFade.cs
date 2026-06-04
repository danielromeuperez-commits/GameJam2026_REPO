using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class QuitGameFade : MonoBehaviour
{
    [Header("Fade References")]
    public CanvasGroup fadeCanvasGroup;
    public Image fadeImage;

    [Header("Fade Settings")]
    public float firstFlashAlpha = 0.25f;
    public float flashDuration = 0.15f;
    public float fadeDuration = 1.2f;
    public float finalHoldTime = 0.35f;

    [Header("Music Fade")]
    public bool fadeMusic = true;
    public float musicFadeDuration = 1.2f;

    [Header("Extra Feel")]
    public bool usePulse = true;
    public float pulseScale = 1.08f;

    private bool isQuitting;

    private void Start()
    {
        if (fadeCanvasGroup != null)
        {
            fadeCanvasGroup.alpha = 0f;
            fadeCanvasGroup.gameObject.SetActive(false);
            fadeCanvasGroup.blocksRaycasts = false;
            fadeCanvasGroup.interactable = false;
        }

        if (fadeImage != null)
            fadeImage.color = Color.black;
    }

    public void QuitGame()
    {
        if (isQuitting) return;

        isQuitting = true;
        StartCoroutine(QuitRoutine());
    }

    private IEnumerator QuitRoutine()
    {
        if (fadeCanvasGroup == null)
        {
            ExitGame();
            yield break;
        }

        fadeCanvasGroup.gameObject.SetActive(true);
        fadeCanvasGroup.blocksRaycasts = true;
        fadeCanvasGroup.interactable = true;

        RectTransform fadeRect = fadeCanvasGroup.GetComponent<RectTransform>();

        if (fadeRect != null)
            fadeRect.localScale = Vector3.one;

        if (fadeMusic)
            StartCoroutine(FadeOutMusic());

        yield return StartCoroutine(FadeTo(firstFlashAlpha, flashDuration));
        yield return StartCoroutine(FadeTo(0.05f, flashDuration));

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

        yield return new WaitForSeconds(finalHoldTime);

        ExitGame();
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

    private IEnumerator FadeTo(float targetAlpha, float duration)
    {
        float startAlpha = fadeCanvasGroup.alpha;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;

            fadeCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);

            yield return null;
        }

        fadeCanvasGroup.alpha = targetAlpha;
    }

    private void ExitGame()
    {
#if UNITY_EDITOR
        Debug.Log("Quit Game pressed. In a build, the game would close now.");
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}