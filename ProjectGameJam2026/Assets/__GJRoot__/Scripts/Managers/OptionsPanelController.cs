using System.Collections;
using UnityEngine;

public class OptionsPanelController : MonoBehaviour
{
    [Header("Panel")]
    public GameObject optionsPanel;

    [Header("Animation")]
    public float hiddenScale = 0.05f;
    public float visibleScale = 1f;
    public float animationDuration = 0.25f;

    private bool isOpen;
    private bool isAnimating;
    private Coroutine currentRoutine;
    private CanvasGroup panelCanvasGroup;

    private void Awake()
    {
        if (optionsPanel != null)
        {
            panelCanvasGroup = optionsPanel.GetComponent<CanvasGroup>();

            if (panelCanvasGroup == null)
                panelCanvasGroup = optionsPanel.AddComponent<CanvasGroup>();
        }

        HideInstant();
    }

    public void ToggleOptions()
    {
        if (isAnimating)
            return;

        if (isOpen)
            CloseOptions();
        else
            OpenOptions();
    }

    public void OpenOptions()
    {
        if (optionsPanel == null)
            return;

        if (isAnimating || isOpen)
            return;

        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(OpenRoutine());
    }

    public void CloseOptions()
    {
        if (optionsPanel == null)
            return;

        if (isAnimating || !isOpen)
            return;

        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(CloseRoutine());
    }

    private IEnumerator OpenRoutine()
    {
        isAnimating = true;
        isOpen = true;

        optionsPanel.SetActive(true);

        if (panelCanvasGroup != null)
        {
            panelCanvasGroup.alpha = 1f;
            panelCanvasGroup.interactable = false;
            panelCanvasGroup.blocksRaycasts = false;
        }

        Vector3 startScale = Vector3.one * hiddenScale;
        Vector3 endScale = Vector3.one * visibleScale;

        optionsPanel.transform.localScale = startScale;

        float timer = 0f;

        while (timer < animationDuration)
        {
            timer += Time.deltaTime;

            float t = timer / animationDuration;
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            optionsPanel.transform.localScale = Vector3.Lerp(startScale, endScale, smoothT);

            yield return null;
        }

        optionsPanel.transform.localScale = endScale;

        if (panelCanvasGroup != null)
        {
            panelCanvasGroup.interactable = true;
            panelCanvasGroup.blocksRaycasts = true;
        }

        isAnimating = false;
        currentRoutine = null;
    }

    private IEnumerator CloseRoutine()
    {
        isAnimating = true;

        if (panelCanvasGroup != null)
        {
            panelCanvasGroup.interactable = false;
            panelCanvasGroup.blocksRaycasts = false;
        }

        Vector3 startScale = optionsPanel.transform.localScale;
        Vector3 endScale = Vector3.one * hiddenScale;

        float timer = 0f;

        while (timer < animationDuration)
        {
            timer += Time.deltaTime;

            float t = timer / animationDuration;
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            optionsPanel.transform.localScale = Vector3.Lerp(startScale, endScale, smoothT);

            yield return null;
        }

        optionsPanel.transform.localScale = endScale;

        yield return new WaitForSeconds(0.05f);

        optionsPanel.SetActive(false);

        isOpen = false;
        isAnimating = false;
        currentRoutine = null;
    }

    private void HideInstant()
    {
        if (optionsPanel == null)
            return;

        optionsPanel.transform.localScale = Vector3.one * hiddenScale;
        optionsPanel.SetActive(false);

        if (panelCanvasGroup != null)
        {
            panelCanvasGroup.alpha = 1f;
            panelCanvasGroup.interactable = false;
            panelCanvasGroup.blocksRaycasts = false;
        }

        isOpen = false;
        isAnimating = false;
    }
}