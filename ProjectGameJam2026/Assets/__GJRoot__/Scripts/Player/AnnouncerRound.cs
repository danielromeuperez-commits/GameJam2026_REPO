using System.Collections;
using TMPro;
using UnityEngine;

public class AnnouncerRound : MonoBehaviour
{
    [Header("Round UI")]
    [SerializeField] private TMP_Text roundText;
    [SerializeField] private GameObject panel;

    [Header("Round Settings")]
    [SerializeField] private float showTime = 4f;

    [Header("Gameplay UI To Hide")]
    [SerializeField] private GameObject uiCanvas1;
    [SerializeField] private GameObject uiCanvas2;
    [SerializeField] private GameObject uiCanvas3;

    private int currentRound = 0;

    public IEnumerator ShowNextRound()
    {
        currentRound++;

        // Ocultar UI del gameplay
        if (uiCanvas1 != null)
            uiCanvas1.SetActive(false);

        if (uiCanvas2 != null)
            uiCanvas2.SetActive(false);

        if (uiCanvas3 != null)
            uiCanvas3.SetActive(false);

        // Mostrar anuncio
        if (panel != null)
            panel.SetActive(true);

        if (roundText != null)
            roundText.text = $"ROUND {currentRound}";

        yield return new WaitForSeconds(showTime);

        // Ocultar anuncio
        if (panel != null)
            panel.SetActive(false);

        // Volver a mostrar UI del gameplay
        if (uiCanvas1 != null)
            uiCanvas1.SetActive(true);

        if (uiCanvas2 != null)
            uiCanvas2.SetActive(true);

        if (uiCanvas3 != null)
            uiCanvas3.SetActive(true);
    }
}