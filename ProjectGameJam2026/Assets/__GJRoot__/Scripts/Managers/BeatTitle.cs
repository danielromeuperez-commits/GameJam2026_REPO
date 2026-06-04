using UnityEngine;

public class BeatTitle : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource audioSource;
    public float bpm = 95f;

    [Header("Pulse")]
    public float escalaBeat = 1.2f;
    public float velocidadVuelta = 10f;

    private Vector3 escalaOriginal;
    private float siguienteBeat;

    void Start()
    {
        escalaOriginal = transform.localScale;
        siguienteBeat = 0f;
    }

    void Update()
    {
        if (!audioSource.isPlaying)
            return;

        float tiempoCancion = audioSource.time;

        if (tiempoCancion >= siguienteBeat)
        {
            transform.localScale = escalaOriginal * escalaBeat;
            siguienteBeat += 60f / bpm;
        }

        transform.localScale = Vector3.Lerp(
            transform.localScale,
            escalaOriginal,
            Time.deltaTime * velocidadVuelta
        );
    }
}