using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonSFX : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    [Header("SFX Index")]
    public int hoverSFXIndex = 0;
    public int clickSFXIndex = 1;

    [Header("Settings")]
    public bool playHoverSound = true;
    public bool playClickSound = true;

    [Header("Pitch Random")]
    public bool randomPitch = true;
    public float minPitch = 0.95f;
    public float maxPitch = 1.05f;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!playHoverSound) return;

        PlaySFX(hoverSFXIndex);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!playClickSound) return;

        PlaySFX(clickSFXIndex);
    }

    private void PlaySFX(int index)
    {
        if (AudioManager.Instance == null)
            return;

        if (randomPitch)
            AudioManager.Instance.PlaySFXRandomPitch(index, minPitch, maxPitch);
        else
            AudioManager.Instance.PlaySFX(index);
    }
}