using UnityEngine;
using TMPro;
using System.Collections;

public class DialogueShuffle : MonoBehaviour
{
    [Header("Buttons")]
    public TextMeshProUGUI textScreen1;
    public TextMeshProUGUI textScreen2;
    public TextMeshProUGUI textScreen3;

    [Header("Question")]
    public TextMeshProUGUI questionText;

    [Header("Conversation Sets")]
    public ConversationSet[] conversationSets;

    private ConversationSet currentSet;
    private int lastSetIndex = -1;

    /// Recibe el orden REAL generado por el sistema de batalla.
    /// El shuffle sigue funcionando exactamente igual.
    public void ActualizarTextosBotones(
        ConversationBattleSystem.ConversationType[] elMapeoReal)
    {
        if (elMapeoReal == null || elMapeoReal.Length < 3)
            return;

        StopAllCoroutines();
        StartCoroutine(RetardoDibujoRonda(elMapeoReal));
    }

    private IEnumerator RetardoDibujoRonda(
        ConversationBattleSystem.ConversationType[] elMapeoReal)
    {
        yield return new WaitForEndOfFrame();

        if (conversationSets == null || conversationSets.Length == 0)
        {
            Debug.LogWarning("No hay Conversation Sets configurados.");
            yield break;
        }

        SeleccionarNuevoConjunto();

        if (questionText != null)
            questionText.text = currentSet.pregunta;

        // MANTENEMOS EL SHUFFLE
        SetButtonText(textScreen1, elMapeoReal[0]);
        SetButtonText(textScreen2, elMapeoReal[1]);
        SetButtonText(textScreen3, elMapeoReal[2]);
    }

    private void SeleccionarNuevoConjunto()
    {
        if (conversationSets.Length == 1)
        {
            currentSet = conversationSets[0];
            return;
        }

        int randomIndex;

        do
        {
            randomIndex = Random.Range(0, conversationSets.Length);
        }
        while (randomIndex == lastSetIndex);

        lastSetIndex = randomIndex;
        currentSet = conversationSets[randomIndex];
    }

    private void SetButtonText(
        TextMeshProUGUI textUI,
        ConversationBattleSystem.ConversationType type)
    {
        if (textUI == null || currentSet == null)
            return;

        switch (type)
        {
            case ConversationBattleSystem.ConversationType.Meh:
                textUI.text = currentSet.fraseMeh;
                break;

            case ConversationBattleSystem.ConversationType.Bueno:
                textUI.text = currentSet.fraseGood;
                break;

            case ConversationBattleSystem.ConversationType.MuyBueno:
                textUI.text = currentSet.fraseVeryGood;
                break;

            default:
                textUI.text = "ERROR";
                break;
        }
    }
}