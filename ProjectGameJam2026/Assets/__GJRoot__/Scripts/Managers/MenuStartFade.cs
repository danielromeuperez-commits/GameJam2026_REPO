using System.Collections;
using UnityEngine;

public class MenuStartFade : MonoBehaviour
{
    [Header("Fade")]
    public CanvasGroup fadeCanvasGroup;
    public float fadeInDuration = 1f;
    public float startBlackHoldTime = 0.25f;

    [Header("Fade Feel")]
    public bool usePulse = true;
    public float pulseScale = 1.04f;

    private void Start()
    {
        StartCoroutine(StartFadeRoutine());
    }

    private IEnumerator StartFadeRoutine()
    {
        if (fadeCanvasGroup == null)
            yield break;

        fadeCanvasGroup.gameObject.SetActive(true);
        fadeCanvasGroup.alpha = 1f;
        fadeCanvasGroup.blocksRaycasts = true;
        fadeCanvasGroup.interactable = true;

        RectTransform fadeRect = fadeCanvasGroup.GetComponent<RectTransform>();

        if (fadeRect != null)
            fadeRect.localScale = Vector3.one;

        yield return new WaitForSeconds(startBlackHoldTime);

        float timer = 0f;

        while (timer < fadeInDuration)
        {
            timer += Time.deltaTime;
            float t = timer / fadeInDuration;

            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            fadeCanvasGroup.alpha = Mathf.Lerp(1f, 0f, smoothT);

            if (usePulse && fadeRect != null)
            {
                float pulse = Mathf.Sin(t * Mathf.PI) * (pulseScale - 1f);
                fadeRect.localScale = Vector3.one * (1f + pulse);
            }

            yield return null;
        }

        fadeCanvasGroup.alpha = 0f;

        if (fadeRect != null)
            fadeRect.localScale = Vector3.one;

        fadeCanvasGroup.blocksRaycasts = false;
        fadeCanvasGroup.interactable = false;
        fadeCanvasGroup.gameObject.SetActive(false);
    }
}