using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;

    public static AudioManager Instance
    {
        get
        {
            if (instance == null)
                Debug.LogWarning("No hay AudioManager en la escena!");

            return instance;
        }
    }

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Audio Libraries")]
    public AudioClip[] musicLibrary;
    public AudioClip[] sfxLibrary;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

            // Hace que el AudioManager no se destruya al cambiar de escena
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // Si ya existe uno, destruye el duplicado
            Destroy(gameObject);
        }
    }

    public void PlayMusic(int musicToPlay)
    {
        if (musicSource == null) return;
        if (musicLibrary == null || musicLibrary.Length == 0) return;
        if (musicToPlay < 0 || musicToPlay >= musicLibrary.Length) return;

        AudioClip selectedClip = musicLibrary[musicToPlay];

        // Si ya está sonando esa misma música, no la reinicia
        if (musicSource.clip == selectedClip && musicSource.isPlaying)
        {
            return;
        }

        musicSource.clip = selectedClip;
        musicSource.Play();
    }

    public void PlaySFX(int sfxToPlay)
    {
        if (sfxSource == null) return;
        if (sfxLibrary == null || sfxLibrary.Length == 0) return;
        if (sfxToPlay < 0 || sfxToPlay >= sfxLibrary.Length) return;

        sfxSource.PlayOneShot(sfxLibrary[sfxToPlay]);
    }

    public void StopMusic()
    {
        if (musicSource != null)
            musicSource.Stop();
    }

    public void PauseMusic()
    {
        if (musicSource != null)
            musicSource.Pause();
    }

    public void UnPauseMusic()
    {
        if (musicSource != null)
            musicSource.UnPause();
    }
}