using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class OptionsSliderKeyboardController : MonoBehaviour
{
    [Header("Slider")]
    public Slider volumeSlider;
    public RectTransform sliderTransform;

    [Header("Close")]
    public OptionsPanelController optionsPanelController;

    [Header("Input")]
    public float changeAmount = 0.05f;
    public bool navigationEnabled;

    [Header("Selected Effect")]
    public float selectedScale = 1.1f;
    public float normalScale = 1f;
    public float rotationAmount = 2f;
    public float pulseSpeed = 10f;
    public float shakeAmount = 3f;
    public float shakeDuration = 0.15f;

    private Vector3 startPosition;
    private Coroutine effectRoutine;

    private void Awake()
    {
        if (sliderTransform == null && volumeSlider != null)
            sliderTransform = volumeSlider.GetComponent<RectTransform>();

        if (sliderTransform != null)
            startPosition = sliderTransform.localPosition;
    }

    private void Update()
    {
        if (!navigationEnabled)
            return;

        Keyboard kb = Keyboard.current;
        if (kb == null) return;

        if (kb.aKey.wasPressedThisFrame || kb.leftArrowKey.wasPressedThisFrame)
        {
            ChangeSlider(-changeAmount);
        }

        if (kb.dKey.wasPressedThisFrame || kb.rightArrowKey.wasPressedThisFrame)
        {
            ChangeSlider(changeAmount);
        }

        if (kb.escapeKey.wasPressedThisFrame || kb.backspaceKey.wasPressedThisFrame)
        {
            if (optionsPanelController != null)
                optionsPanelController.CloseOptions();
        }
    }

    public void SetNavigationEnabled(bool enabled)
    {
        navigationEnabled = enabled;

        if (enabled)
        {
            if (volumeSlider != null)
                EventSystem.current.SetSelectedGameObject(volumeSlider.gameObject);

            StartSelectedEffect();
        }
        else
        {
            StopSelectedEffect();
        }
    }

    private void ChangeSlider(float amount)
    {
        if (volumeSlider == null)
            return;

        volumeSlider.value = Mathf.Clamp(
            volumeSlider.value + amount,
            volumeSlider.minValue,
            volumeSlider.maxValue
        );

        PlayModifyFeedback();
    }

    private void StartSelectedEffect()
    {
        if (sliderTransform == null)
            return;

        if (effectRoutine != null)
            StopCoroutine(effectRoutine);

        effectRoutine = StartCoroutine(SelectedEffectRoutine());
    }

    private void StopSelectedEffect()
    {
        if (effectRoutine != null)
        {
            StopCoroutine(effectRoutine);
            effectRoutine = null;
        }

        if (sliderTransform != null)
        {
            sliderTransform.localScale = Vector3.one * normalScale;
            sliderTransform.localRotation = Quaternion.identity;
            sliderTransform.localPosition = startPosition;
        }
    }

    private IEnumerator SelectedEffectRoutine()
    {
        while (true)
        {
            float pulse = Mathf.Sin(Time.time * pulseSpeed) * 0.03f;
            float rotation = Mathf.Sin(Time.time * pulseSpeed) * rotationAmount;

            sliderTransform.localScale = Vector3.one * (selectedScale + pulse);
            sliderTransform.localRotation = Quaternion.Euler(0f, 0f, rotation);

            yield return null;
        }
    }

    private void PlayModifyFeedback()
    {
        if (sliderTransform == null)
            return;

        StartCoroutine(ShakeRoutine());
    }

    private IEnumerator ShakeRoutine()
    {
        float timer = 0f;

        while (timer < shakeDuration)
        {
            timer += Time.deltaTime;

            float x = Random.Range(-shakeAmount, shakeAmount);
            sliderTransform.localPosition = startPosition + new Vector3(x, 0f, 0f);

            yield return null;
        }

        sliderTransform.localPosition = startPosition;
    }
}