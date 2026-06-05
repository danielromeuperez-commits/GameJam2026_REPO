using UnityEngine;

public class BeatTitle : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource audioSource;
    public float bpm = 95f;

    [Header("Objects To Pulse")]
    public Transform[] beatObjects;

    [Header("Pulse")]
    public float escalaBeat = 1.2f;
    public float velocidadVuelta = 10f;

    private Vector3[] escalasOriginales;
    private float siguienteBeat;

    private void Start()
    {
        if (audioSource == null && AudioManager.Instance != null)
            audioSource = AudioManager.Instance.musicSource;

        if (beatObjects == null || beatObjects.Length == 0)
        {
            beatObjects = new Transform[1];
            beatObjects[0] = transform;
        }

        escalasOriginales = new Vector3[beatObjects.Length];

        for (int i = 0; i < beatObjects.Length; i++)
        {
            if (beatObjects[i] != null)
                escalasOriginales[i] = beatObjects[i].localScale;
        }

        siguienteBeat = 0f;
    }

    private void Update()
    {
        if (audioSource == null)
            return;

        if (!audioSource.isPlaying)
            return;

        float tiempoCancion = audioSource.time;

        if (tiempoCancion >= siguienteBeat)
        {
            PulseObjects();
            siguienteBeat += 60f / bpm;
        }

        ReturnToOriginalScale();
    }

    private void PulseObjects()
    {
        for (int i = 0; i < beatObjects.Length; i++)
        {
            if (beatObjects[i] == null)
                continue;

            beatObjects[i].localScale = escalasOriginales[i] * escalaBeat;
        }
    }

    private void ReturnToOriginalScale()
    {
        for (int i = 0; i < beatObjects.Length; i++)
        {
            if (beatObjects[i] == null)
                continue;

            beatObjects[i].localScale = Vector3.Lerp(
                beatObjects[i].localScale,
                escalasOriginales[i],
                Time.deltaTime * velocidadVuelta
            );
        }
    }
}