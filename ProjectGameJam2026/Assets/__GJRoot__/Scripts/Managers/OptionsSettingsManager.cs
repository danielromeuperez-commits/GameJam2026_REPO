using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsSettingsManager : MonoBehaviour
{
    [Header("Volume Sliders")]
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;

    [Header("Fullscreen")]
    public Button fullscreenButton;
    public TMP_Text fullscreenButtonText;

    [Header("Screen Shake")]
    public Button screenShakeButton;
    public TMP_Text screenShakeButtonText;

    [Header("How To Play")]
    public GameObject howToPlayPanel;

    [Header("How To Play Transition")]
    public RectTransform howToPlayTransitionPanel;
    public CanvasGroup howToPlayTransitionCanvasGroup;
    public float transitionDuration = 0.25f;
    public float transitionHoldTime = 0.5f;
    public float transitionDistance = 2200f;

    private bool fullscreenEnabled;
    private bool screenShakeEnabled;
    private bool isTransitioning;

    public static bool ScreenShakeEnabled { get; private set; } = true;

    private void Start()
    {
        LoadSettings();
        SetupSliders();
        UpdateFullscreenText();
        UpdateScreenShakeText();

        if (howToPlayPanel != null)
            howToPlayPanel.SetActive(false);

        if (howToPlayTransitionPanel != null)
            howToPlayTransitionPanel.gameObject.SetActive(false);
    }

    private void LoadSettings()
    {
        fullscreenEnabled = PlayerPrefs.GetInt("Fullscreen", Screen.fullScreen ? 1 : 0) == 1;
        screenShakeEnabled = PlayerPrefs.GetInt("ScreenShake", 1) == 1;

        ApplyFullscreen();
        ScreenShakeEnabled = screenShakeEnabled;
    }

    private void SetupSliders()
    {
        if (AudioManager.Instance == null)
            return;

        if (masterVolumeSlider != null)
        {
            masterVolumeSlider.value = AudioManager.Instance.GetMasterVolume();
            masterVolumeSlider.onValueChanged.RemoveAllListeners();
            masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        }

        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.value = AudioManager.Instance.GetMusicVolume();
            musicVolumeSlider.onValueChanged.RemoveAllListeners();
            musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
        }

        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.value = AudioManager.Instance.GetSFXVolume();
            sfxVolumeSlider.onValueChanged.RemoveAllListeners();
            sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);
        }
    }

    public void SetMasterVolume(float value)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetMasterVolume(value);
    }

    public void SetMusicVolume(float value)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetMusicVolume(value);
    }

    public void SetSFXVolume(float value)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetSFXVolume(value);
    }

    public void ToggleFullscreen()
    {
        fullscreenEnabled = !fullscreenEnabled;

        ApplyFullscreen();

        PlayerPrefs.SetInt("Fullscreen", fullscreenEnabled ? 1 : 0);
        UpdateFullscreenText();
    }

    private void ApplyFullscreen()
    {
        if (fullscreenEnabled)
        {
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
            Screen.fullScreen = true;
        }
        else
        {
            Screen.fullScreenMode = FullScreenMode.Windowed;
            Screen.fullScreen = false;
        }
    }

    public void ToggleScreenShake()
    {
        screenShakeEnabled = !screenShakeEnabled;

        ScreenShakeEnabled = screenShakeEnabled;
        PlayerPrefs.SetInt("ScreenShake", screenShakeEnabled ? 1 : 0);

        UpdateScreenShakeText();
    }

    public void OpenHowToPlay()
    {
        if (isTransitioning) return;

        StartCoroutine(OpenHowToPlayRoutine());
    }

    public void CloseHowToPlay()
    {
        if (isTransitioning) return;

        StartCoroutine(CloseHowToPlayRoutine());
    }

    private IEnumerator OpenHowToPlayRoutine()
    {
        isTransitioning = true;

        // Entra el negro de derecha a izquierda
        yield return StartCoroutine(PlayBlackSwipeIn(true));

        // Espera con la pantalla negra
        yield return new WaitForSeconds(transitionHoldTime);

        // Cambia a la pantalla How To Play mientras está negro
        if (howToPlayPanel != null)
            howToPlayPanel.SetActive(true);

        // Sale el negro hacia la izquierda y revela el How To Play
        yield return StartCoroutine(PlayBlackSwipeOut(true));

        isTransitioning = false;
    }

    private IEnumerator CloseHowToPlayRoutine()
    {
        isTransitioning = true;

        // Entra el negro de izquierda a derecha
        yield return StartCoroutine(PlayBlackSwipeIn(false));

        // Espera con la pantalla negra
        yield return new WaitForSeconds(transitionHoldTime);

        // Oculta el How To Play mientras está negro
        if (howToPlayPanel != null)
            howToPlayPanel.SetActive(false);

        // Sale el negro hacia la derecha y revela Options
        yield return StartCoroutine(PlayBlackSwipeOut(false));

        isTransitioning = false;
    }

    private IEnumerator PlayBlackSwipeIn(bool rightToLeft)
    {
        if (howToPlayTransitionPanel == null)
            yield break;

        howToPlayTransitionPanel.gameObject.SetActive(true);

        if (howToPlayTransitionCanvasGroup != null)
            howToPlayTransitionCanvasGroup.alpha = 1f;

        Vector2 startPosition;
        Vector2 centerPosition = Vector2.zero;

        if (rightToLeft)
            startPosition = new Vector2(transitionDistance, 0f);
        else
            startPosition = new Vector2(-transitionDistance, 0f);

        howToPlayTransitionPanel.anchoredPosition = startPosition;

        float timer = 0f;

        while (timer < transitionDuration)
        {
            timer += Time.deltaTime;
            float t = timer / transitionDuration;
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            howToPlayTransitionPanel.anchoredPosition = Vector2.Lerp(
                startPosition,
                centerPosition,
                smoothT
            );

            yield return null;
        }

        howToPlayTransitionPanel.anchoredPosition = centerPosition;
    }

    private IEnumerator PlayBlackSwipeOut(bool rightToLeft)
    {
        if (howToPlayTransitionPanel == null)
            yield break;

        Vector2 centerPosition = Vector2.zero;
        Vector2 endPosition;

        if (rightToLeft)
            endPosition = new Vector2(-transitionDistance, 0f);
        else
            endPosition = new Vector2(transitionDistance, 0f);

        howToPlayTransitionPanel.anchoredPosition = centerPosition;

        float timer = 0f;

        while (timer < transitionDuration)
        {
            timer += Time.deltaTime;
            float t = timer / transitionDuration;
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            howToPlayTransitionPanel.anchoredPosition = Vector2.Lerp(
                centerPosition,
                endPosition,
                smoothT
            );

            yield return null;
        }

        howToPlayTransitionPanel.anchoredPosition = endPosition;
        howToPlayTransitionPanel.gameObject.SetActive(false);
    }

    private void UpdateFullscreenText()
    {
        if (fullscreenButtonText != null)
            fullscreenButtonText.text = fullscreenEnabled ? "ON" : "OFF";
    }

    private void UpdateScreenShakeText()
    {
        if (screenShakeButtonText != null)
            screenShakeButtonText.text = screenShakeEnabled ? "ON" : "OFF";
    }
}