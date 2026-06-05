using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenuController : MonoBehaviour
{
    [Header("Pause Panel")]
    public GameObject pausePanel;
    public CanvasGroup pauseCanvasGroup;

    [Header("Pause Animation")]
    public float hiddenScale = 0.05f;
    public float visibleScale = 1f;
    public float animationDuration = 0.2f;

    [Header("Black Fade")]
    public CanvasGroup blackFadeCanvasGroup;
    public float blackFadeDuration = 0.8f;
    public float blackHoldTime = 0.15f;

    [Header("SFX")]
    public int openPauseSFXIndex = 1;
    public int closePauseSFXIndex = 1;
    public int buttonClickSFXIndex = 1;

    [Header("Music Fade")]
    public bool fadeMusicOnQuit = true;
    public float musicFadeDuration = 0.8f;

    private bool isPaused;
    private bool isAnimating;
    private bool isExiting;
    private Coroutine currentRoutine;

    private void Awake()
    {
        if (pauseCanvasGroup == null && pausePanel != null)
            pauseCanvasGroup = pausePanel.GetComponent<CanvasGroup>();

        if (pauseCanvasGroup == null && pausePanel != null)
            pauseCanvasGroup = pausePanel.AddComponent<CanvasGroup>();

        SetupBlackFade();
        HideInstant();
    }

    private void Update()
    {
        if (isExiting)
            return;

        Keyboard kb = Keyboard.current;

        if (kb == null)
            return;

        if (kb.escapeKey.wasPressedThisFrame)
        {
            TogglePause();
        }
    }

    private void SetupBlackFade()
    {
        if (blackFadeCanvasGroup == null)
            return;

        blackFadeCanvasGroup.gameObject.SetActive(false);
        blackFadeCanvasGroup.alpha = 0f;
        blackFadeCanvasGroup.blocksRaycasts = false;
        blackFadeCanvasGroup.interactable = false;
    }

    public void TogglePause()
    {
        if (isAnimating || isExiting)
            return;

        if (isPaused)
            ResumeGame();
        else
            PauseGame();
    }

    public void PauseGame()
    {
        if (pausePanel == null)
            return;

        if (isAnimating || isPaused || isExiting)
            return;

        PlaySFX(openPauseSFXIndex);

        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(OpenPauseRoutine());
    }

    public void ResumeGame()
    {
        if (pausePanel == null)
            return;

        if (isAnimating || !isPaused || isExiting)
            return;

        PlaySFX(closePauseSFXIndex);

        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(ClosePauseRoutine());
    }

    private IEnumerator OpenPauseRoutine()
    {
        isAnimating = true;
        isPaused = true;

        pausePanel.SetActive(true);
        Time.timeScale = 0f;

        if (pauseCanvasGroup != null)
        {
            pauseCanvasGroup.alpha = 1f;
            pauseCanvasGroup.interactable = false;
            pauseCanvasGroup.blocksRaycasts = false;
        }

        Vector3 startScale = Vector3.one * hiddenScale;
        Vector3 endScale = Vector3.one * visibleScale;

        pausePanel.transform.localScale = startScale;

        float timer = 0f;

        while (timer < animationDuration)
        {
            timer += Time.unscaledDeltaTime;

            float t = timer / animationDuration;
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            pausePanel.transform.localScale = Vector3.Lerp(startScale, endScale, smoothT);

            yield return null;
        }

        pausePanel.transform.localScale = endScale;

        if (pauseCanvasGroup != null)
        {
            pauseCanvasGroup.interactable = true;
            pauseCanvasGroup.blocksRaycasts = true;
        }

        isAnimating = false;
        currentRoutine = null;
    }

    private IEnumerator ClosePauseRoutine()
    {
        isAnimating = true;

        if (pauseCanvasGroup != null)
        {
            pauseCanvasGroup.interactable = false;
            pauseCanvasGroup.blocksRaycasts = false;
        }

        Vector3 startScale = pausePanel.transform.localScale;
        Vector3 endScale = Vector3.one * hiddenScale;

        float timer = 0f;

        while (timer < animationDuration)
        {
            timer += Time.unscaledDeltaTime;

            float t = timer / animationDuration;
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            pausePanel.transform.localScale = Vector3.Lerp(startScale, endScale, smoothT);

            yield return null;
        }

        pausePanel.transform.localScale = endScale;
        pausePanel.SetActive(false);

        Time.timeScale = 1f;

        isPaused = false;
        isAnimating = false;
        currentRoutine = null;
    }

    public void QuitGame()
    {
        if (isExiting)
            return;

        StartCoroutine(QuitGameRoutine());
    }

    private IEnumerator QuitGameRoutine()
    {
        isExiting = true;

        PlaySFX(buttonClickSFXIndex);

        if (pauseCanvasGroup != null)
        {
            pauseCanvasGroup.interactable = false;
            pauseCanvasGroup.blocksRaycasts = false;
        }

        if (fadeMusicOnQuit)
            StartCoroutine(FadeOutMusic());

        yield return StartCoroutine(BlackFadeIn());

        yield return new WaitForSecondsRealtime(blackHoldTime);

        Time.timeScale = 1f;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopMusic();
            AudioManager.Instance.ResetMusicFadeMultiplier();
            AudioManager.Instance.RefreshAudioVolumes();
        }

#if UNITY_EDITOR
        Debug.Log("Quit Game pressed. In build, the game would close.");
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private IEnumerator BlackFadeIn()
    {
        if (blackFadeCanvasGroup == null)
            yield break;

        blackFadeCanvasGroup.gameObject.SetActive(true);
        blackFadeCanvasGroup.blocksRaycasts = true;
        blackFadeCanvasGroup.interactable = true;
        blackFadeCanvasGroup.alpha = 0f;

        float timer = 0f;

        while (timer < blackFadeDuration)
        {
            timer += Time.unscaledDeltaTime;

            float t = timer / blackFadeDuration;
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            blackFadeCanvasGroup.alpha = smoothT;

            yield return null;
        }

        blackFadeCanvasGroup.alpha = 1f;
    }

    private IEnumerator FadeOutMusic()
    {
        if (AudioManager.Instance == null)
            yield break;

        float timer = 0f;

        while (timer < musicFadeDuration)
        {
            timer += Time.unscaledDeltaTime;

            float t = timer / musicFadeDuration;
            float fadeValue = Mathf.Lerp(1f, 0f, t);

            AudioManager.Instance.SetMusicFadeMultiplier(fadeValue);

            yield return null;
        }

        AudioManager.Instance.SetMusicFadeMultiplier(0f);
    }

    private void HideInstant()
    {
        if (pausePanel == null)
            return;

        pausePanel.transform.localScale = Vector3.one * hiddenScale;
        pausePanel.SetActive(false);

        if (pauseCanvasGroup != null)
        {
            pauseCanvasGroup.alpha = 1f;
            pauseCanvasGroup.interactable = false;
            pauseCanvasGroup.blocksRaycasts = false;
        }

        Time.timeScale = 1f;

        isPaused = false;
        isAnimating = false;
        isExiting = false;
    }

    private void PlaySFX(int index)
    {
        if (AudioManager.Instance == null)
            return;

        AudioManager.Instance.PlaySFXRandomPitch(index, 0.95f, 1.05f);
    }
}