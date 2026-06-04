using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class KeyboardMenuNavigator : MonoBehaviour
{
    [Header("Buttons In Order")]
    public Button[] menuButtons;

    [Header("Settings")]
    public bool selectFirstButtonOnStart = true;
    public bool navigationEnabled = true;

    private int currentIndex;

    private void Start()
    {
        if (selectFirstButtonOnStart && navigationEnabled)
            SelectButton(0);
    }

    private void Update()
    {
        if (!navigationEnabled)
            return;

        Keyboard kb = Keyboard.current;
        if (kb == null) return;

        if (kb.wKey.wasPressedThisFrame || kb.upArrowKey.wasPressedThisFrame)
        {
            MoveSelection(-1);
        }

        if (kb.sKey.wasPressedThisFrame || kb.downArrowKey.wasPressedThisFrame)
        {
            MoveSelection(1);
        }

        if (kb.enterKey.wasPressedThisFrame || kb.spaceKey.wasPressedThisFrame)
        {
            PressCurrentButton();
        }
    }

    public void SetNavigationEnabled(bool enabled)
    {
        navigationEnabled = enabled;

        if (!enabled)
        {
            EventSystem.current.SetSelectedGameObject(null);
            return;
        }

        SelectButton(currentIndex);
    }

    private void MoveSelection(int direction)
    {
        if (menuButtons == null || menuButtons.Length == 0)
            return;

        currentIndex += direction;

        if (currentIndex < 0)
            currentIndex = menuButtons.Length - 1;

        if (currentIndex >= menuButtons.Length)
            currentIndex = 0;

        SelectButton(currentIndex);
    }

    private void SelectButton(int index)
    {
        if (menuButtons == null || menuButtons.Length == 0)
            return;

        if (index < 0 || index >= menuButtons.Length)
            return;

        currentIndex = index;

        EventSystem.current.SetSelectedGameObject(menuButtons[currentIndex].gameObject);
    }

    private void PressCurrentButton()
    {
        if (menuButtons == null || menuButtons.Length == 0)
            return;

        if (currentIndex < 0 || currentIndex >= menuButtons.Length)
            return;

        menuButtons[currentIndex].onClick.Invoke();
    }
}