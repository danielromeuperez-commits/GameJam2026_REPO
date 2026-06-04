using System.Collections;
using UnityEngine;

public class UIButtonAppearFromBottom : MonoBehaviour
{
    [Header("References")]
    public RectTransform target;
    public CanvasGroup canvasGroup;

    [Header("Appear Movement")]
    public float startYOffset = -80f;
    public float appearDuration = 0.35f;
    public float delay = 0f;

    [Header("Disappear Movement")]
    public float disappearYOffset = -80f;
    public float disappearDuration = 0.25f;

    [Header("Scale")]
    public float startScale = 0.9f;
    public float overshootScale = 1.05f;
    public float normalScale = 1f;
    public float disappearScale = 0.9f;

    private Vector2 originalPosition;
    private Vector3 originalScale;
    private Coroutine currentRoutine;

    private void Awake()
    {
        if (target == null)
            target = GetComponent<RectTransform>();

        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        originalPosition = target.anchoredPosition;
        originalScale = target.localScale;
    }

    private void OnEnable()
    {
        PlayAppear();
    }

    public void PlayAppear()
    {
        if (target == null)
            return;

        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(AppearRoutine());
    }

    public IEnumerator PlayDisappearRoutine()
    {
        if (target == null)
            yield break;

        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(DisappearRoutine());

        yield return currentRoutine;
    }

    private IEnumerator AppearRoutine()
    {
        canvasGroup.alpha = 0f;

        target.anchoredPosition = originalPosition + new Vector2(0f, startYOffset);
        target.localScale = originalScale * startScale;

        if (delay > 0f)
            yield return new WaitForSeconds(delay);

        float timer = 0f;

        while (timer < appearDuration)
        {
            timer += Time.deltaTime;
            float t = timer / appearDuration;
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            canvasGroup.alpha = smoothT;

            target.anchoredPosition = Vector2.Lerp(
                originalPosition + new Vector2(0f, startYOffset),
                originalPosition,
                smoothT
            );

            float scale;

            if (t < 0.75f)
            {
                float popT = t / 0.75f;
                scale = Mathf.Lerp(startScale, overshootScale, Mathf.SmoothStep(0f, 1f, popT));
            }
            else
            {
                float settleT = (t - 0.75f) / 0.25f;
                scale = Mathf.Lerp(overshootScale, normalScale, Mathf.SmoothStep(0f, 1f, settleT));
            }

            target.localScale = originalScale * scale;

            yield return null;
        }

        canvasGroup.alpha = 1f;
        target.anchoredPosition = originalPosition;
        target.localScale = originalScale * normalScale;

        currentRoutine = null;
    }

    private IEnumerator DisappearRoutine()
    {
        canvasGroup.alpha = 1f;

        Vector2 startPosition = target.anchoredPosition;
        Vector2 endPosition = originalPosition + new Vector2(0f, disappearYOffset);

        Vector3 startScaleValue = target.localScale;
        Vector3 endScaleValue = originalScale * disappearScale;

        float timer = 0f;

        while (timer < disappearDuration)
        {
            timer += Time.deltaTime;
            float t = timer / disappearDuration;
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            canvasGroup.alpha = Mathf.Lerp(1f, 0f, smoothT);

            target.anchoredPosition = Vector2.Lerp(
                startPosition,
                endPosition,
                smoothT
            );

            target.localScale = Vector3.Lerp(
                startScaleValue,
                endScaleValue,
                smoothT
            );

            yield return null;
        }

        canvasGroup.alpha = 0f;
        target.anchoredPosition = endPosition;
        target.localScale = endScaleValue;

        currentRoutine = null;
    }
}