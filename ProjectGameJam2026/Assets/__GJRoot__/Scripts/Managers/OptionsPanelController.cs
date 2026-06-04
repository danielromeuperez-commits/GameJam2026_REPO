using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class OptionsPanelController : MonoBehaviour
{
    [Header("Options Panel")]
    public RectTransform optionsPanel;

    [Header("Navigators")]
    public KeyboardMenuNavigator mainMenuNavigator;
    public OptionsSliderKeyboardController optionsSliderController;

    [Header("Scale Animation")]
    public Vector3 hiddenScale = new Vector3(0.1f, 0.1f, 0.1f);
    public Vector3 visibleScale = Vector3.one;
    public float scaleSpeed = 8f;

    private bool isOpen;
    private Coroutine scaleRoutine;

    private void Start()
    {
        HideInstant();
    }

    public void OpenOptions()
    {
        if (isOpen) return;

        isOpen = true;

        if (mainMenuNavigator != null)
            mainMenuNavigator.SetNavigationEnabled(false);

        if (scaleRoutine != null)
            StopCoroutine(scaleRoutine);

        optionsPanel.gameObject.SetActive(true);
        scaleRoutine = StartCoroutine(OpenPanelRoutine());
    }

    public void CloseOptions()
    {
        if (!isOpen) return;

        isOpen = false;

        if (optionsSliderController != null)
            optionsSliderController.SetNavigationEnabled(false);

        if (scaleRoutine != null)
            StopCoroutine(scaleRoutine);

        scaleRoutine = StartCoroutine(ClosePanelRoutine());
    }

    private IEnumerator OpenPanelRoutine()
    {
        yield return StartCoroutine(ScalePanel(visibleScale));

        if (optionsSliderController != null)
            optionsSliderController.SetNavigationEnabled(true);
    }

    private IEnumerator ClosePanelRoutine()
    {
        yield return StartCoroutine(ScalePanel(hiddenScale));

        optionsPanel.gameObject.SetActive(false);

        if (mainMenuNavigator != null)
            mainMenuNavigator.SetNavigationEnabled(true);
    }

    private IEnumerator ScalePanel(Vector3 targetScale)
    {
        if (optionsPanel == null)
            yield break;

        while (Vector3.Distance(optionsPanel.localScale, targetScale) > 0.01f)
        {
            optionsPanel.localScale = Vector3.Lerp(
                optionsPanel.localScale,
                targetScale,
                Time.deltaTime * scaleSpeed
            );

            yield return null;
        }

        optionsPanel.localScale = targetScale;
    }

    private void HideInstant()
    {
        if (optionsPanel == null)
            return;

        optionsPanel.localScale = hiddenScale;
        optionsPanel.gameObject.SetActive(false);
        isOpen = false;

        if (optionsSliderController != null)
            optionsSliderController.SetNavigationEnabled(false);
    }
}