using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuButtonJuice : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    ISelectHandler,
    IDeselectHandler
{
    public enum HoverMoveDirection
    {
        Horizontal,
        Vertical,
        None
    }

    [Header("Button Visuals")]
    public RectTransform buttonTransform;
    public TMP_Text buttonText;
    public Image buttonImage;

    [Header("Hover Scale")]
    public float normalScaleMultiplier = 1f;
    public float selectedScaleMultiplier = 1.15f;

    [Header("Hover Rotation")]
    public float selectedRotation = -3f;

    [Header("Hover Movement")]
    public HoverMoveDirection hoverMoveDirection = HoverMoveDirection.Horizontal;
    public float selectedXOffset = 25f;
    public float selectedYOffset = 20f;

    [Header("Text Colors")]
    public Color selectedTextColor = Color.black;
    public Color dimTextColor = new Color(0.15f, 0.15f, 0.15f, 1f);

    [Header("Button Image Colors")]
    public bool changeButtonImageColor = false;
    public Color selectedImageColor = Color.white;
    public Color dimImageColor = new Color(0.65f, 0.65f, 0.65f, 1f);

    [Header("Smooth Animation")]
    public float animationSpeed = 10f;

    [Header("Beat Pulse While Hovering")]
    public AudioSource audioSource;
    public float bpm = 95f;
    public float beatScale = 1.12f;
    public float beatReturnSpeed = 10f;

    private Vector3 startPosition;
    private Vector3 startScale;
    private Quaternion startRotation;

    private Vector3 targetPosition;
    private Vector3 targetBaseScale;
    private Quaternion targetRotation;

    private bool isHovering;
    private float nextBeat;
    private float beatMultiplier = 1f;

    private void Awake()
    {
        if (buttonTransform == null)
            buttonTransform = GetComponent<RectTransform>();

        if (buttonText == null)
            buttonText = GetComponentInChildren<TMP_Text>();

        if (buttonImage == null)
            buttonImage = GetComponent<Image>();

        if (audioSource == null && AudioManager.Instance != null)
            audioSource = AudioManager.Instance.musicSource;

        startPosition = buttonTransform.localPosition;
        startScale = buttonTransform.localScale;
        startRotation = buttonTransform.localRotation;

        targetPosition = startPosition;
        targetBaseScale = startScale * normalScaleMultiplier;
        targetRotation = startRotation;

        SetDimmedInstant();
    }

    private void Update()
    {
        UpdateBeatPulse();

        Vector3 finalScale = targetBaseScale * beatMultiplier;

        buttonTransform.localPosition = Vector3.Lerp(
            buttonTransform.localPosition,
            targetPosition,
            Time.deltaTime * animationSpeed
        );

        buttonTransform.localScale = Vector3.Lerp(
            buttonTransform.localScale,
            finalScale,
            Time.deltaTime * animationSpeed
        );

        buttonTransform.localRotation = Quaternion.Lerp(
            buttonTransform.localRotation,
            targetRotation,
            Time.deltaTime * animationSpeed
        );
    }

    private void UpdateBeatPulse()
    {
        if (isHovering && audioSource != null && audioSource.isPlaying)
        {
            float songTime = audioSource.time;

            if (songTime >= nextBeat)
            {
                beatMultiplier = beatScale;
                nextBeat += 60f / bpm;
            }
        }

        beatMultiplier = Mathf.Lerp(
            beatMultiplier,
            1f,
            Time.deltaTime * beatReturnSpeed
        );
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;

        if (audioSource == null && AudioManager.Instance != null)
            audioSource = AudioManager.Instance.musicSource;

        if (audioSource != null)
            nextBeat = audioSource.time;

        EventSystem.current.SetSelectedGameObject(gameObject);
        SetSelected();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        beatMultiplier = 1f;

        if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject == gameObject)
            EventSystem.current.SetSelectedGameObject(null);

        SetDimmed();
    }

    public void OnSelect(BaseEventData eventData)
    {
        SetSelected();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        if (!isHovering)
            SetDimmed();
    }

    private void SetSelected()
    {
        if (buttonText != null)
            buttonText.color = selectedTextColor;

        if (changeButtonImageColor && buttonImage != null)
            buttonImage.color = selectedImageColor;

        targetBaseScale = startScale * selectedScaleMultiplier;
        targetRotation = startRotation * Quaternion.Euler(0f, 0f, selectedRotation);

        switch (hoverMoveDirection)
        {
            case HoverMoveDirection.Horizontal:
                targetPosition = startPosition + new Vector3(selectedXOffset, 0f, 0f);
                break;

            case HoverMoveDirection.Vertical:
                targetPosition = startPosition + new Vector3(0f, selectedYOffset, 0f);
                break;

            case HoverMoveDirection.None:
                targetPosition = startPosition;
                break;
        }
    }

    private void SetDimmed()
    {
        if (buttonText != null)
            buttonText.color = dimTextColor;

        if (changeButtonImageColor && buttonImage != null)
            buttonImage.color = dimImageColor;

        targetBaseScale = startScale * normalScaleMultiplier;
        targetPosition = startPosition;
        targetRotation = startRotation;
    }

    private void SetDimmedInstant()
    {
        if (buttonText != null)
            buttonText.color = dimTextColor;

        if (changeButtonImageColor && buttonImage != null)
            buttonImage.color = dimImageColor;

        buttonTransform.localPosition = startPosition;
        buttonTransform.localScale = startScale * normalScaleMultiplier;
        buttonTransform.localRotation = startRotation;
    }
}