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
    public float typedWaveScale = 1.28f;
    public float successScale = 1.45f;

    [Header("Active Movement")]
    public float moveAmount = 6f;
    public float moveSpeed = 8f;

    [Header("Typed Letter Wave")]
    public float typedWaveHeight = 10f;
    public float typedWaveDuration = 0.28f;

    [Header("Wrong Animation")]
    public float wrongShakeAmount = 8f;
    public float wrongAnimationTime = 0.25f;

    [Header("Success Wave")]
    public float successJumpHeight = 16f;
    public float successJumpDuration = 0.45f;

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

    public void SetLetter(string value)
    {
        gameObject.SetActive(true);

        if (letterText != null)
            letterText.text = value;

        SetNormal();
    }

    public void HideLetter()
    {
        gameObject.SetActive(false);
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

    public void SetWrong()
    {
        StopCurrentRoutine();

        isActive = false;
        transform.localPosition = startPosition;
        transform.localScale = Vector3.one * normalScale;

        if (letterText != null)
            letterText.color = wrongColor;
    }

    public void PlayTypedWave()
    {
        StopCurrentRoutine();
        currentRoutine = StartCoroutine(TypedWaveRoutine());
    }

    public void PlayWrongFeedback()
    {
        StopCurrentRoutine();
        currentRoutine = StartCoroutine(WrongRoutine());
    }

    public void PlaySuccessWave(float delay)
    {
        StopCurrentRoutine();
        currentRoutine = StartCoroutine(SuccessWaveRoutine(delay));
    }

    private IEnumerator TypedWaveRoutine()
    {
        isActive = false;

        if (letterText != null)
            letterText.color = completedColor;

        float timer = 0f;

        while (timer < typedWaveDuration)
        {
            timer += Time.deltaTime;
            float t = timer / typedWaveDuration;

            float wave = Mathf.Sin(t * Mathf.PI);
            float y = wave * typedWaveHeight;
            float scale = Mathf.Lerp(normalScale, typedWaveScale, wave);

            transform.localPosition = startPosition + new Vector3(0f, y, 0f);
            transform.localScale = Vector3.one * scale;

            yield return null;
        }

        transform.localPosition = startPosition;
        transform.localScale = Vector3.one * normalScale;

        if (letterText != null)
            letterText.color = completedColor;

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

        currentRoutine = null;
    }

    private IEnumerator SuccessWaveRoutine(float delay)
    {
        isActive = false;

        yield return new WaitForSeconds(delay);

        if (letterText != null)
            letterText.color = successColor;

        float timer = 0f;

        while (timer < successJumpDuration)
        {
            timer += Time.deltaTime;
            float t = timer / successJumpDuration;

            float wave = Mathf.Sin(t * Mathf.PI);
            float y = wave * successJumpHeight;
            float scale = Mathf.Lerp(normalScale, successScale, wave);

            transform.localPosition = startPosition + new Vector3(0f, y, 0f);
            transform.localScale = Vector3.one * scale;

            yield return null;
        }

        transform.localPosition = startPosition;
        transform.localScale = Vector3.one * normalScale;

        if (letterText != null)
            letterText.color = successColor;

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