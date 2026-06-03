using UnityEngine;

public class ShuffleBotones : MonoBehaviour
{
    public RectTransform[] botones;

    void Start()
    {
        // Guardamos posiciones reales antes de tocar nada
        Vector3[] posiciones = new Vector3[botones.Length];

        for (int i = 0; i < botones.Length; i++)
        {
            posiciones[i] = botones[i].position;
        }

        // Mezclar índices
        for (int i = 0; i < botones.Length; i++)
        {
            int r = Random.Range(i, botones.Length);

            // swap posiciones en array
            Vector3 temp = posiciones[i];
            posiciones[i] = posiciones[r];
            posiciones[r] = temp;
        }

        // Aplicar posiciones en WORLD SPACE (clave aquí)
        for (int i = 0; i < botones.Length; i++)
        {
            botones[i].position = posiciones[i];
        }
    }
}