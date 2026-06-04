using System.Collections;
using TMPro;
using UnityEngine;

public class AnnouncerRound : MonoBehaviour
{
    [Header("Round UI")]
    [SerializeField] private TMP_Text roundText;
    [SerializeField] private GameObject panel;
    [SerializeField] private CanvasGroup panelCanvasGroup;

    [Header("Round Settings")]
    [SerializeField] private float showTime = 4f;

    [Header("Health UI")]
    [SerializeField] private GameObject healthCanvas;
    [SerializeField] private HealthBarJuice player1HealthBar;
    [SerializeField] private HealthBarJuice player2HealthBar;

    [Header("Intro Animation")]
    [SerializeField] private float introDuration = 0.45f;
    [SerializeField] private float startScale = 0.25f;
    [SerializeField] private float overshootScale = 1.35f;
    [SerializeField] private float normalScale = 1f;

    [Header("Outro Animation")]
    [SerializeField] private float outroDuration = 0.35f;
    [SerializeField] private float exitScale = 0.15f;

    [Header("Beat Pulse")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private float bpm = 95f;
    [SerializeField] private float beatScale = 1.18f;
    [SerializeField] private float beatReturnSpeed = 10f;

    [Header("Gameplay UI To Hide")]
    [SerializeField] private GameObject uiCanvas1;
    [SerializeField] private GameObject uiCanvas2;
    [SerializeField] private GameObject uiCanvas3;

    private int currentRound = 0;

    private Vector3 originalTextScale;
    private float nextBeat;
    private float beatMultiplier = 1f;

    private bool healthBarsAlreadyAppeared;

    private void Awake()
    {
        if (roundText != null)
            originalTextScale = roundText.transform.localScale;

        if (panelCanvasGroup == null && panel != null)
            panelCanvasGroup = panel.GetComponent<CanvasGroup>();

        if (panelCanvasGroup == null && panel != null)
            panelCanvasGroup = panel.AddComponent<CanvasGroup>();

        if (panel != null)
            panel.SetActive(false);

        // Solo ocultamos las barras de vida al inicio.
        // Los otros canvas los controla el flujo normal de la ronda.
        if (healthCanvas != null)
            healthCanvas.SetActive(false);
    }

    public IEnumerator ShowNextRound()
    {
        currentRound++;

        HideGameplayUI();

        if (healthCanvas != null && !healthBarsAlreadyAppeared)
            healthCanvas.SetActive(false);

        if (panel != null)
            panel.SetActive(true);

        if (roundText != null)
        {
            roundText.text = $"ROUND {currentRound}";
            roundText.transform.localScale = originalTextScale * startScale;
        }

        if (panelCanvasGroup != null)
            panelCanvasGroup.alpha = 0f;

        if (audioSource != null)
            nextBeat = audioSource.time;

        yield return StartCoroutine(IntroAnimation());

        float timer = 0f;

        while (timer < showTime)
        {
            timer += Time.deltaTime;
            UpdateBeatPulse();
            yield return null;
        }

        yield return StartCoroutine(OutroAnimation());

        if (panel != null)
            panel.SetActive(false);

        ShowGameplayUI();

        if (!healthBarsAlreadyAppeared)
        {
            healthBarsAlreadyAppeared = true;
            StartCoroutine(ShowHealthBarsWithFade());
        }
        else
        {
            if (healthCanvas != null)
                healthCanvas.SetActive(true);
        }
    }

    private IEnumerator ShowHealthBarsWithFade()
    {
        if (healthCanvas != null)
            healthCanvas.SetActive(true);

        // Esperamos 1 frame para que Unity reactive bien el Canvas.
        yield return null;

        if (player1HealthBar != null)
            player1HealthBar.PlayAppear();

        if (player2HealthBar != null)
            player2HealthBar.PlayAppear();
    }

    private IEnumerator IntroAnimation()
    {
        float timer = 0f;

        while (timer < introDuration)
        {
            timer += Time.deltaTime;
            float t = timer / introDuration;

            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            if (panelCanvasGroup != null)
                panelCanvasGroup.alpha = smoothT;

            if (roundText != null)
            {
                float scale;

                if (t < 0.65f)
                {
                    float popT = t / 0.65f;
                    scale = Mathf.Lerp(startScale, overshootScale, Mathf.SmoothStep(0f, 1f, popT));
                }
                else
                {
                    float settleT = (t - 0.65f) / 0.35f;
                    scale = Mathf.Lerp(overshootScale, normalScale, Mathf.SmoothStep(0f, 1f, settleT));
                }

                roundText.transform.localScale = originalTextScale * scale;
            }

            yield return null;
        }

        if (panelCanvasGroup != null)
            panelCanvasGroup.alpha = 1f;

        if (roundText != null)
            roundText.transform.localScale = originalTextScale * normalScale;
    }

    private void UpdateBeatPulse()
    {
        if (roundText == null)
            return;

        if (audioSource != null && audioSource.isPlaying)
        {
            float songTime = audioSource.time;

            if (songTime >= nextBeat)
            {
                beatMultiplier = beatScale;
                nextBeat += 60f / bpm;
            }
        }

        beatMultiplier = Mathf.Lerp(
            beatMultiplier,
            1f,
            Time.deltaTime * beatReturnSpeed
        );

        roundText.transform.localScale = originalTextScale * normalScale * beatMultiplier;
    }

    private IEnumerator OutroAnimation()
    {
        float timer = 0f;

        Vector3 startTextScale = roundText != null ? roundText.transform.localScale : Vector3.one;

        while (timer < outroDuration)
        {
            timer += Time.deltaTime;
            float t = timer / outroDuration;

            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            if (panelCanvasGroup != null)
                panelCanvasGroup.alpha = Mathf.Lerp(1f, 0f, smoothT);

            if (roundText != null)
            {
                roundText.transform.localScale = Vector3.Lerp(
                    startTextScale,
                    originalTextScale * exitScale,
                    smoothT
                );
            }

            yield return null;
        }

        if (panelCanvasGroup != null)
            panelCanvasGroup.alpha = 0f;
    }

    private void HideGameplayUI()
    {
        if (uiCanvas1 != null)
            uiCanvas1.SetActive(false);

        if (uiCanvas2 != null)
            uiCanvas2.SetActive(false);

        if (uiCanvas3 != null)
            uiCanvas3.SetActive(false);
    }

    private void ShowGameplayUI()
    {
        if (uiCanvas1 != null)
            uiCanvas1.SetActive(true);

        if (uiCanvas2 != null)
            uiCanvas2.SetActive(true);

        if (uiCanvas3 != null)
            uiCanvas3.SetActive(true);
    }
}