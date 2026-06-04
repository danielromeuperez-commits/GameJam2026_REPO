using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SliderJuice : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("References")]
    public RectTransform sliderTransform;
    public RectTransform handleTransform;
    public Slider slider;

    [Header("Volume Text")]
    public RectTransform volumeTextTransform;
    public TMP_Text volumeText;

    [Header("Slider Scale Multipliers")]
    public float normalScaleMultiplier = 1f;
    public float hoverScaleMultiplier = 1.02f;
    public float draggingScaleMultiplier = 1.04f;

    [Header("Volume Text Pulse")]
    public float textPulseAmount = 0.025f;
    public float textPulseSpeed = 3f;

    [Header("Handle Rotation")]
    public float handleRotationAmount = 4f;

    [Header("Pulse While Dragging")]
    public float pulseAmount = 0.006f;
    public float pulseSpeed = 5f;

    [Header("Animation")]
    public float animationSpeed = 8f;

    private bool isHovering;
    private bool isDragging;

    private Vector3 startScale;
    private Vector3 startPosition;
    private Vector3 textStartScale;
    private Quaternion startHandleRotation;

    private Coroutine scaleRoutine;

    private void Awake()
    {
        if (slider == null)
            slider = GetComponent<Slider>();

        if (sliderTransform == null)
            sliderTransform = GetComponent<RectTransform>();

        if (volumeText != null && volumeTextTransform == null)
            volumeTextTransform = volumeText.GetComponent<RectTransform>();

        if (sliderTransform != null)
        {
            startScale = sliderTransform.localScale;
            startPosition = sliderTransform.localPosition;
        }

        if (volumeTextTransform != null)
            textStartScale = volumeTextTransform.localScale;

        if (handleTransform != null)
            startHandleRotation = handleTransform.localRotation;
    }

    private void Update()
    {
        AnimateVolumeText();

        if (!isDragging || sliderTransform == null)
            return;

        float pulse = Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
        sliderTransform.localScale = startScale * (draggingScaleMultiplier + pulse);

        if (handleTransform != null)
        {
            float rotation = Mathf.Sin(Time.time * pulseSpeed) * handleRotationAmount;
            handleTransform.localRotation = startHandleRotation * Quaternion.Euler(0f, 0f, rotation);
        }
    }

    private void AnimateVolumeText()
    {
        if (volumeTextTransform == null)
            return;

        float pulse = Mathf.Sin(Time.time * textPulseSpeed) * textPulseAmount;
        volumeTextTransform.localScale = textStartScale * (1f + pulse);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;

        if (!isDragging)
            AnimateScale(hoverScaleMultiplier);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;

        if (!isDragging)
            AnimateScale(normalScaleMultiplier);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isDragging = true;

        if (scaleRoutine != null)
            StopCoroutine(scaleRoutine);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;

        if (handleTransform != null)
            handleTransform.localRotation = startHandleRotation;

        if (sliderTransform != null)
            sliderTransform.localPosition = startPosition;

        if (isHovering)
            AnimateScale(hoverScaleMultiplier);
        else
            AnimateScale(normalScaleMultiplier);
    }

    private void AnimateScale(float targetMultiplier)
    {
        if (sliderTransform == null)
            return;

        if (scaleRoutine != null)
            StopCoroutine(scaleRoutine);

        scaleRoutine = StartCoroutine(ScaleRoutine(targetMultiplier));
    }

    private IEnumerator ScaleRoutine(float targetMultiplier)
    {
        Vector3 targetScale = startScale * targetMultiplier;

        while (Vector3.Distance(sliderTransform.localScale, targetScale) > 0.01f)
        {
            sliderTransform.localScale = Vector3.Lerp(
                sliderTransform.localScale,
                targetScale,
                Time.deltaTime * animationSpeed
            );

            yield return null;
        }

        sliderTransform.localScale = targetScale;
    }
}