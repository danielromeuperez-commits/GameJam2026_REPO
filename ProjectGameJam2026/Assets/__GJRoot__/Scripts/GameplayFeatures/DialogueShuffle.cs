using UnityEngine;
using TMPro;
using System.Collections; // Necesario para la corrutina de retardo

public class ShuffleBotones : MonoBehaviour
{
    public TextMeshProUGUI textScreen1;
    public TextMeshProUGUI textScreen2;
    public TextMeshProUGUI textScreen3;

    [Header("Phrases Pool")]
    public string[] phraseMeh;
    public string[] phraseGood;
    public string[] phraseVeryGood;

    /// <summary>
    /// Recibe el nuevo orden del sistema de batalla y espera un microsegundo 
    /// para dibujarlo, asegurando que el orden físico cambie por completo.
    /// </summary>
    public void ActualizarTextosBotones(ConversationBattleSystem.ConversationType[] elMapeoReal)
    {
        if (elMapeoReal == null || elMapeoReal.Length < 3) return;

        // Detenemos cualquier actualización previa y lanzamos la nueva con un mini-retraso seguro
        StopAllCoroutines();
        StartCoroutine(RetardoDibujoRonda(elMapeoReal));
    }

    private IEnumerator RetardoDibujoRonda(ConversationBattleSystem.ConversationType[] elMapeoReal)
    {
        // Esperamos al final del frame actual para que la lógica de batalla se asiente
        yield return new WaitForEndOfFrame();

        // Ahora sí asignamos de manera real y desordenada a cada pantalla
        SetButtonText(textScreen1, elMapeoReal[0]);
        SetButtonText(textScreen2, elMapeoReal[1]);
        SetButtonText(textScreen3, elMapeoReal[2]);
    }

    private void SetButtonText(TextMeshProUGUI textUI, ConversationBattleSystem.ConversationType type)
    {
        if (textUI == null) return;

        string[] sourceArray = GetArray(type);

        if (sourceArray == null || sourceArray.Length == 0)
        {
            textUI.text = "EMPTY";
            return;
        }

        // Elige una frase totalmente aleatoria del array correspondiente
        int index = Random.Range(0, sourceArray.Length);
        textUI.text = sourceArray[index];
    }

    private string[] GetArray(ConversationBattleSystem.ConversationType type)
    {
        return type switch
        {
            ConversationBattleSystem.ConversationType.Meh => phraseMeh,
            ConversationBattleSystem.ConversationType.Bueno => phraseGood,
            ConversationBattleSystem.ConversationType.MuyBueno => phraseVeryGood,
            _ => null
        };
    }
}