using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuButtonJuice : MonoBehaviour, IPointerEnterHandler, ISelectHandler, IDeselectHandler
{
    [Header("Button Visuals")]
    public RectTransform buttonTransform;
    public TMP_Text buttonText;
    public Image buttonImage;

    [Header("Scale")]
    public float normalScale = 1f;
    public float selectedScale = 1.15f;

    [Header("Rotation")]
    public float selectedRotation = -3f;

    [Header("Movement")]
    public float selectedXOffset = 25f;

    [Header("Text Colors")]
    public Color selectedTextColor = Color.black;
    public Color dimTextColor = new Color(0.15f, 0.15f, 0.15f, 1f);

    [Header("Button Image Colors")]
    public bool changeButtonImageColor = false;
    public Color selectedImageColor = Color.white;
    public Color dimImageColor = new Color(0.65f, 0.65f, 0.65f, 1f);

    [Header("Animation")]
    public float animationSpeed = 10f;

    private Vector3 startPosition;
    private Coroutine animationRoutine;

    private void Awake()
    {
        if (buttonTransform == null)
            buttonTransform = GetComponent<RectTransform>();

        if (buttonText == null)
            buttonText = GetComponentInChildren<TMP_Text>();

        if (buttonImage == null)
            buttonImage = GetComponent<Image>();

        startPosition = buttonTransform.localPosition;

        SetDimmed();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        EventSystem.current.SetSelectedGameObject(gameObject);
    }

    public void OnSelect(BaseEventData eventData)
    {
        SetSelected();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        SetDimmed();
    }

    public void SetSelected()
    {
        if (buttonText != null)
            buttonText.color = selectedTextColor;

        if (changeButtonImageColor && buttonImage != null)
            buttonImage.color = selectedImageColor;

        StartButtonAnimation(
            Vector3.one * selectedScale,
            startPosition + new Vector3(selectedXOffset, 0f, 0f),
            Quaternion.Euler(0f, 0f, selectedRotation)
        );
    }

    public void SetDimmed()
    {
        if (buttonText != null)
            buttonText.color = dimTextColor;

        if (changeButtonImageColor && buttonImage != null)
            buttonImage.color = dimImageColor;

        StartButtonAnimation(
            Vector3.one * normalScale,
            startPosition,
            Quaternion.identity
        );
    }

    private void StartButtonAnimation(Vector3 targetScale, Vector3 targetPosition, Quaternion targetRotation)
    {
        if (buttonTransform == null) return;

        if (animationRoutine != null)
            StopCoroutine(animationRoutine);

        animationRoutine = StartCoroutine(AnimateButton(targetScale, targetPosition, targetRotation));
    }

    private IEnumerator AnimateButton(Vector3 targetScale, Vector3 targetPosition, Quaternion targetRotation)
    {
        while (
            Vector3.Distance(buttonTransform.localScale, targetScale) > 0.01f ||
            Vector3.Distance(buttonTransform.localPosition, targetPosition) > 0.01f ||
            Quaternion.Angle(buttonTransform.localRotation, targetRotation) > 0.1f
        )
        {
            buttonTransform.localScale = Vector3.Lerp(
                buttonTransform.localScale,
                targetScale,
                Time.deltaTime * animationSpeed
            );

            buttonTransform.localPosition = Vector3.Lerp(
                buttonTransform.localPosition,
                targetPosition,
                Time.deltaTime * animationSpeed
            );

            buttonTransform.localRotation = Quaternion.Lerp(
                buttonTransform.localRotation,
                targetRotation,
                Time.deltaTime * animationSpeed
            );

            yield return null;
        }

        buttonTransform.localScale = targetScale;
        buttonTransform.localPosition = targetPosition;
        buttonTransform.localRotation = targetRotation;
    }
}