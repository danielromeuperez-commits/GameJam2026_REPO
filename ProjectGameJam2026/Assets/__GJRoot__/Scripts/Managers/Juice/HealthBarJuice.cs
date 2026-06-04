using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarJuice : MonoBehaviour
{
    [Header("References")]
    public Image healthFillImage;
    public RectTransform barTransform;
    public CanvasGroup canvasGroup;

    [Header("Appear Fade")]
    public float appearDuration = 0.6f;
    public float appearDelay = 0f;

    [Header("Health Change")]
    public float fillLerpSpeed = 8f;

    [Header("Damage Feedback")]
    public Color normalColor = Color.white;
    public Color damageColor = new Color(1f, 0.15f, 0.15f, 1f);
    public float damageFlashTime = 0.18f;
    public float shakeAmount = 6f;
    public float shakeTime = 0.18f;
    public float damagePunchScale = 1.06f;

    private float targetFillAmount = 1f;

    private Vector3 originalPosition;
    private Vector3 originalScale;

    private Coroutine appearRoutine;
    private Coroutine damageRoutine;

    private void Awake()
    {
        if (barTransform == null)
            barTransform = GetComponent<RectTransform>();

        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        if (barTransform != null)
        {
            originalPosition = barTransform.localPosition;
            originalScale = barTransform.localScale;
        }

        if (healthFillImage != null)
        {
            healthFillImage.type = Image.Type.Filled;
            healthFillImage.fillMethod = Image.FillMethod.Horizontal;
            healthFillImage.fillOrigin = 0;
            healthFillImage.fillAmount = 1f;
            healthFillImage.color = normalColor;
        }
    }

    private void Update()
    {
        if (healthFillImage == null)
            return;

        healthFillImage.fillAmount = Mathf.Lerp(
            healthFillImage.fillAmount,
            targetFillAmount,
            Time.deltaTime * fillLerpSpeed
        );
    }

    public void SetHealth01Instant(float value)
    {
        targetFillAmount = Mathf.Clamp01(value);

        if (healthFillImage != null)
            healthFillImage.fillAmount = targetFillAmount;
    }

    public void SetHealth01(float value)
    {
        float newFill = Mathf.Clamp01(value);

        bool tookDamage = newFill < targetFillAmount;

        targetFillAmount = newFill;

        if (tookDamage)
            PlayDamageFeedback();
    }

    public void PlayAppear()
    {
        if (appearRoutine != null)
            StopCoroutine(appearRoutine);

        appearRoutine = StartCoroutine(AppearRoutine());
    }

    private IEnumerator AppearRoutine()
    {
        if (canvasGroup == null)
            yield break;

        canvasGroup.alpha = 0f;

        if (appearDelay > 0f)
            yield return new WaitForSeconds(appearDelay);

        float timer = 0f;

        while (timer < appearDuration)
        {
            timer += Time.deltaTime;
            float t = timer / appearDuration;
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            canvasGroup.alpha = smoothT;

            yield return null;
        }

        canvasGroup.alpha = 1f;
    }

    public void PlayDamageFeedback()
    {
        if (damageRoutine != null)
            StopCoroutine(damageRoutine);

        damageRoutine = StartCoroutine(DamageRoutine());
    }

    private IEnumerator DamageRoutine()
    {
        if (barTransform == null)
            yield break;

        if (healthFillImage != null)
            healthFillImage.color = damageColor;

        float timer = 0f;

        while (timer < shakeTime)
        {
            timer += Time.deltaTime;
            float t = timer / shakeTime;

            float shakeX = Random.Range(-shakeAmount, shakeAmount);
            float shakeY = Random.Range(-shakeAmount * 0.35f, shakeAmount * 0.35f);

            barTransform.localPosition = originalPosition + new Vector3(shakeX, shakeY, 0f);

            float punch = Mathf.Sin(t * Mathf.PI);
            barTransform.localScale = Vector3.Lerp(
                originalScale,
                originalScale * damagePunchScale,
                punch
            );

            yield return null;
        }

        barTransform.localPosition = originalPosition;
        barTransform.localScale = originalScale;

        float colorTimer = 0f;

        while (colorTimer < damageFlashTime)
        {
            colorTimer += Time.deltaTime;
            float t = colorTimer / damageFlashTime;

            if (healthFillImage != null)
            {
                healthFillImage.color = Color.Lerp(
                    damageColor,
                    normalColor,
                    t
                );
            }

            yield return null;
        }

        if (healthFillImage != null)
            healthFillImage.color = normalColor;

        damageRoutine = null;
    }
}