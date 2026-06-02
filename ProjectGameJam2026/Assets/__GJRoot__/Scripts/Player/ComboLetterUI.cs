using System.Collections;
using TMPro;
using UnityEngine;

public class ComboLetterUI : MonoBehaviour
{
    public TMP_Text letterText;

    [Header("Colors")]
    public Color normalColor = Color.gray;
    public Color activeColor = Color.white;
    public Color completedColor = Color.yellow;
    public Color wrongColor = Color.red;
    public Color successColor = Color.cyan;

    [Header("Scale")]
    public float normalScale = 1f;
    public float activeScale = 1.2f;
    public float wrongScale = 1.35f;
    public float successScale = 1.35f;

    [Header("Movement")]
    public float moveAmount = 6f;
    public float moveSpeed = 8f;

    [Header("Wrong Animation")]
    public float wrongShakeAmount = 8f;
    public float wrongAnimationTime = 0.25f;

    [Header("Success Animation")]
    public float successJumpHeight = 12f;
    public float successJumpSpeed = 3.5f;
    public int successJumps = 3;

    private Vector3 startPosition;
    private bool isActive;
    private Coroutine currentRoutine;

    private void Awake()
    {
        if (letterText == null)
            letterText = GetComponent<TMP_Text>();

        startPosition = transform.localPosition;
        SetNormal();
    }

    private void Update()
    {
        if (!isActive) return;

        float offset = Mathf.Sin(Time.time * moveSpeed) * moveAmount;
        transform.localPosition = startPosition + new Vector3(0f, offset, 0f);
    }

    public void SetNormal()
    {
        StopCurrentRoutine();

        isActive = false;
        transform.localPosition = startPosition;
        transform.localScale = Vector3.one * normalScale;

        if (letterText != null)
            letterText.color = normalColor;
    }

    public void SetActive()
    {
        StopCurrentRoutine();

        isActive = true;
        transform.localScale = Vector3.one * activeScale;

        if (letterText != null)
            letterText.color = activeColor;
    }

    public void SetCompleted()
    {
        StopCurrentRoutine();

        isActive = false;
        transform.localPosition = startPosition;
        transform.localScale = Vector3.one * normalScale;

        if (letterText != null)
            letterText.color = completedColor;
    }

    public void PlayCorrectPop()
    {
        StopCurrentRoutine();
        currentRoutine = StartCoroutine(CorrectPopRoutine());
    }

    public void PlayWrongFeedback()
    {
        StopCurrentRoutine();
        currentRoutine = StartCoroutine(WrongRoutine());
    }

    public void PlaySuccessBounce()
    {
        StopCurrentRoutine();
        currentRoutine = StartCoroutine(SuccessBounceRoutine());
    }

    private IEnumerator CorrectPopRoutine()
    {
        isActive = false;

        if (letterText != null)
            letterText.color = completedColor;

        float timer = 0f;
        float duration = 0.18f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float t = timer / duration;
            float scale = Mathf.Lerp(activeScale, normalScale, t);

            transform.localScale = Vector3.one * scale;
            transform.localPosition = startPosition;

            yield return null;
        }

        transform.localScale = Vector3.one * normalScale;
        transform.localPosition = startPosition;
        currentRoutine = null;
    }

    private IEnumerator WrongRoutine()
    {
        isActive = false;

        if (letterText != null)
            letterText.color = wrongColor;

        transform.localScale = Vector3.one * wrongScale;

        float timer = 0f;

        while (timer < wrongAnimationTime)
        {
            timer += Time.deltaTime;

            float shakeX = Random.Range(-wrongShakeAmount, wrongShakeAmount);
            transform.localPosition = startPosition + new Vector3(shakeX, 0f, 0f);

            yield return null;
        }

        transform.localPosition = startPosition;
        transform.localScale = Vector3.one * normalScale;

        if (letterText != null)
            letterText.color = normalColor;

        currentRoutine = null;
    }

    private IEnumerator SuccessBounceRoutine()
    {
        isActive = false;

        if (letterText != null)
            letterText.color = successColor;

        for (int i = 0; i < successJumps; i++)
        {
            float timer = 0f;

            while (timer < 1f)
            {
                timer += Time.deltaTime * successJumpSpeed;

                float jump = Mathf.Sin(timer * Mathf.PI) * successJumpHeight;
                float scaleBoost = Mathf.Sin(timer * Mathf.PI) * (successScale - normalScale);

                transform.localPosition = startPosition + new Vector3(0f, jump, 0f);
                transform.localScale = Vector3.one * (normalScale + scaleBoost);

                yield return null;
            }

            transform.localPosition = startPosition;
            transform.localScale = Vector3.one * normalScale;
        }

        currentRoutine = null;
    }

    private void StopCurrentRoutine()
    {
        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
            currentRoutine = null;
        }
    }
}