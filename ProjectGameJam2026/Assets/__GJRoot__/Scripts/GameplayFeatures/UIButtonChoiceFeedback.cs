using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIButtonChoiceFeedback : MonoBehaviour
{
    [Header("References")]
    public RectTransform target;
    public Image backgroundImage;
    public CanvasGroup canvasGroup;

    [Header("Selected Feedback")]
    public float selectedScale = 1.12f;
    public float punchScale = 1.22f;
    public float animationDuration = 0.18f;

    [Header("Dimmed")]
    public float dimmedAlpha = 0.45f;

    [Header("Colors")]
    public bool changeColor = true;
    public Color normalColor = Color.white;
    public Color selectedColor = new Color(1f, 0.9f, 0.25f, 1f);

    private Vector3 originalScale;
    private Coroutine currentRoutine;

    private void Awake()
    {
        if (target == null)
            target = GetComponent<RectTransform>();

        if (backgroundImage == null)
            backgroundImage = GetComponent<Image>();

        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        if (target != null)
            originalScale = target.localScale;
    }

    public void SetNormal()
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        if (target != null)
            target.localScale = originalScale;

        if (canvasGroup != null)
            canvasGroup.alpha = 1f;

        if (backgroundImage != null && changeColor)
            backgroundImage.color = normalColor;
    }

    public void SetDimmed()
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        if (target != null)
            target.localScale = originalScale;

        if (canvasGroup != null)
            canvasGroup.alpha = dimmedAlpha;

        if (backgroundImage != null && changeColor)
            backgroundImage.color = normalColor;
    }

    public void PlaySelected()
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(SelectedRoutine());
    }

    private IEnumerator SelectedRoutine()
    {
        if (canvasGroup != null)
            canvasGroup.alpha = 1f;

        if (backgroundImage != null && changeColor)
            backgroundImage.color = selectedColor;

        if (target == null)
            yield break;

        Vector3 startScale = target.localScale;
        Vector3 punch = originalScale * punchScale;
        Vector3 selected = originalScale * selectedScale;

        float halfDuration = animationDuration * 0.5f;
        float timer = 0f;

        while (timer < halfDuration)
        {
            timer += Time.deltaTime;
            float t = timer / halfDuration;
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            target.localScale = Vector3.Lerp(startScale, punch, smoothT);

            yield return null;
        }

        timer = 0f;

        while (timer < halfDuration)
        {
            timer += Time.deltaTime;
            float t = timer / halfDuration;
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            target.localScale = Vector3.Lerp(punch, selected, smoothT);

            yield return null;
        }

        target.localScale = selected;
        currentRoutine = null;
    }
}