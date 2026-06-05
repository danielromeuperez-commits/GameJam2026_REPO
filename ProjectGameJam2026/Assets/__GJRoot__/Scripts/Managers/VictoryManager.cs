using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryManager : MonoBehaviour
{
    public static VictoryManager Instance;

    [Header("Victory UI")]
    public GameObject victoryPanel;
    public CanvasGroup victoryCanvasGroup;
    public TMP_Text victoryText;

    [Header("Settings")]
    public string mainMenuSceneName = "MainMenu";
    public float timeBeforeReturn = 4f;
    public float fadeDuration = 0.5f;

    private bool victoryTriggered;

    private void Awake()
    {
        Instance = this;

        if (victoryCanvasGroup == null && victoryPanel != null)
            victoryCanvasGroup = victoryPanel.GetComponent<CanvasGroup>();

        if (victoryCanvasGroup == null && victoryPanel != null)
            victoryCanvasGroup = victoryPanel.AddComponent<CanvasGroup>();

        HideVictoryInstant();
    }

    public void ShowVictory(int winnerPlayer)
    {
        if (victoryTriggered)
            return;

        victoryTriggered = true;

        StartCoroutine(VictoryRoutine(winnerPlayer));
    }

    private IEnumerator VictoryRoutine(int winnerPlayer)
    {
        Time.timeScale = 1f;

        if (victoryPanel != null)
            victoryPanel.SetActive(true);

        if (victoryText != null)
            victoryText.text = "PLAYER " + winnerPlayer + " WINS!";

        if (victoryCanvasGroup != null)
        {
            victoryCanvasGroup.alpha = 0f;
            victoryCanvasGroup.interactable = true;
            victoryCanvasGroup.blocksRaycasts = true;
        }

        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;

            float t = timer / fadeDuration;
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            if (victoryCanvasGroup != null)
                victoryCanvasGroup.alpha = smoothT;

            yield return null;
        }

        if (victoryCanvasGroup != null)
            victoryCanvasGroup.alpha = 1f;

        yield return new WaitForSeconds(timeBeforeReturn);

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopMusic();
            AudioManager.Instance.ResetMusicFadeMultiplier();
            AudioManager.Instance.RefreshAudioVolumes();
        }

        SceneManager.LoadScene(mainMenuSceneName);
    }

    private void HideVictoryInstant()
    {
        if (victoryPanel != null)
            victoryPanel.SetActive(false);

        if (victoryCanvasGroup != null)
        {
            victoryCanvasGroup.alpha = 0f;
            victoryCanvasGroup.interactable = false;
            victoryCanvasGroup.blocksRaycasts = false;
        }
    }
}