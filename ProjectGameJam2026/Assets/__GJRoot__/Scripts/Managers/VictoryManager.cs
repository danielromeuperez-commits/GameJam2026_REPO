using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryManager : MonoBehaviour
{
    public static VictoryManager Instance;

    [Header("Victory Panels")]
    public GameObject player1VictoryPanel;
    public GameObject player2VictoryPanel;

    [Header("Objects To Disable")]
    public GameObject[] objectsToDisable;

    [Header("Object To Destroy")]
    public GameObject objectToDestroy;

    [Header("Settings")]
    public string mainMenuSceneName = "MainMenu";
    public float delayBeforeShow = 1f;
    public float timeBeforeReturn = 4f;
    public float fadeDuration = 0.5f;

    private bool victoryTriggered;

    private CanvasGroup player1CanvasGroup;
    private CanvasGroup player2CanvasGroup;

    private void Awake()
    {
        Instance = this;

        player1CanvasGroup = GetOrCreateCanvasGroup(player1VictoryPanel);
        player2CanvasGroup = GetOrCreateCanvasGroup(player2VictoryPanel);

        HideAllVictoryPanels();
    }

    private CanvasGroup GetOrCreateCanvasGroup(GameObject panel)
    {
        if (panel == null) return null;

        CanvasGroup cg = panel.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = panel.AddComponent<CanvasGroup>();

        return cg;
    }

    public void ShowVictory(int winnerPlayer)
    {
        if (victoryTriggered) return;

        victoryTriggered = true;
        StartCoroutine(VictoryRoutine(winnerPlayer));
    }

    private IEnumerator VictoryRoutine(int winnerPlayer)
    {
        Time.timeScale = 1f;

        yield return new WaitForSeconds(delayBeforeShow);

        DisableObjects();

        DestroyObject();

        GameObject panelToShow = null;
        CanvasGroup canvasToFade = null;

        switch (winnerPlayer)
        {
            case 1:
                panelToShow = player1VictoryPanel;
                canvasToFade = player1CanvasGroup;
                break;

            case 2:
                panelToShow = player2VictoryPanel;
                canvasToFade = player2CanvasGroup;
                break;
        }

        if (panelToShow != null)
            panelToShow.SetActive(true);

        if (canvasToFade != null)
        {
            canvasToFade.alpha = 0f;
            canvasToFade.interactable = true;
            canvasToFade.blocksRaycasts = true;
        }

        AudioManager.Instance?.PlaySFX(7);

        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;

            float t = timer / fadeDuration;
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            if (canvasToFade != null)
                canvasToFade.alpha = smoothT;

            yield return null;
        }

        if (canvasToFade != null)
            canvasToFade.alpha = 1f;

        yield return new WaitForSeconds(timeBeforeReturn);

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopMusic();
            AudioManager.Instance.ResetMusicFadeMultiplier();
            AudioManager.Instance.RefreshAudioVolumes();
        }

        SceneManager.LoadScene(mainMenuSceneName);
    }

    private void DisableObjects()
    {
        foreach (GameObject obj in objectsToDisable)
        {
            if (obj != null)
                obj.SetActive(false);
        }
    }

    private void DestroyObject()
    {
        if (objectToDestroy != null)
            Destroy(objectToDestroy);
    }

    private void HideAllVictoryPanels()
    {
        HidePanel(player1VictoryPanel, player1CanvasGroup);
        HidePanel(player2VictoryPanel, player2CanvasGroup);
    }

    private void HidePanel(GameObject panel, CanvasGroup canvasGroup)
    {
        if (panel != null)
            panel.SetActive(false);

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }
}