using System.Collections;
using UnityEngine;

public class ComboCircleUI : MonoBehaviour
{
    [Header("References")]
    public RectTransform circleTransform;

    [Header("Wrong Feedback")]
    public float shakeDuration = 0.25f;
    public float shakeAmount = 8f;

    private Vector2 originalPosition;
    private Coroutine shakeRoutine;

    private void Awake()
    {
        if (circleTransform == null)
            circleTransform = GetComponent<RectTransform>();

        if (circleTransform != null)
            originalPosition = circleTransform.anchoredPosition;
    }

    public void PlayShake()
    {
        if (circleTransform == null)
            return;

        if (shakeRoutine != null)
            StopCoroutine(shakeRoutine);

        shakeRoutine = StartCoroutine(ShakeRoutine());
    }

    private IEnumerator ShakeRoutine()
    {
        float timer = 0f;

        while (timer < shakeDuration)
        {
            timer += Time.deltaTime;

            float x = Random.Range(-shakeAmount, shakeAmount);
            float y = Random.Range(-shakeAmount, shakeAmount);

            circleTransform.anchoredPosition = originalPosition + new Vector2(x, y);

            yield return null;
        }

        circleTransform.anchoredPosition = originalPosition;
        shakeRoutine = null;
    }
}