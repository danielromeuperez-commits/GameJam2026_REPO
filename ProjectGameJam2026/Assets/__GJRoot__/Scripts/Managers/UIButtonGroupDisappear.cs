using System.Collections;
using UnityEngine;

public class UIButtonGroupDisappear : MonoBehaviour
{
    [Header("Buttons In Disappear Order")]
    public UIButtonAppearFromBottom[] buttons;

    [Header("Timing")]
    public float delayBetweenButtons = 0.06f;

    public IEnumerator PlayDisappearAll()
    {
        if (buttons == null || buttons.Length == 0)
            yield break;

        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i] != null)
                StartCoroutine(buttons[i].PlayDisappearRoutine());

            yield return new WaitForSeconds(delayBetweenButtons);
        }

        float longestDisappear = 0f;

        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i] != null && buttons[i].disappearDuration > longestDisappear)
                longestDisappear = buttons[i].disappearDuration;
        }

        yield return new WaitForSeconds(longestDisappear);
    }
}