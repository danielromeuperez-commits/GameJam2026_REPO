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

    private float masterVolume = 1f;
    private float musicVolume = 1f;
    private float sfxVolume = 1f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            LoadAudioSettings();
            ApplyVolumes();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayMusic(int musicToPlay)
    {
        if (musicSource == null) return;
        if (musicLibrary == null || musicLibrary.Length == 0) return;
        if (musicToPlay < 0 || musicToPlay >= musicLibrary.Length) return;

        AudioClip selectedClip = musicLibrary[musicToPlay];

        if (musicSource.clip == selectedClip && musicSource.isPlaying)
            return;

        musicSource.clip = selectedClip;
        musicSource.Play();
    }

    public void PlaySFX(int sfxToPlay)
    {
        if (sfxSource == null) return;
        if (sfxLibrary == null || sfxLibrary.Length == 0) return;
        if (sfxToPlay < 0 || sfxToPlay >= sfxLibrary.Length) return;

        sfxSource.PlayOneShot(sfxLibrary[sfxToPlay], masterVolume * sfxVolume);
    }

    public void SetMasterVolume(float value)
    {
        masterVolume = Mathf.Clamp01(value);
        PlayerPrefs.SetFloat("MasterVolume", masterVolume);
        ApplyVolumes();
    }

    public void SetMusicVolume(float value)
    {
        musicVolume = Mathf.Clamp01(value);
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        ApplyVolumes();
    }

    public void SetSFXVolume(float value)
    {
        sfxVolume = Mathf.Clamp01(value);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        ApplyVolumes();
    }

    public float GetMasterVolume()
    {
        return masterVolume;
    }

    public float GetMusicVolume()
    {
        return musicVolume;
    }

    public float GetSFXVolume()
    {
        return sfxVolume;
    }

    private void ApplyVolumes()
    {
        if (musicSource != null)
            musicSource.volume = masterVolume * musicVolume;

        if (sfxSource != null)
            sfxSource.volume = masterVolume * sfxVolume;
    }

    private void LoadAudioSettings()
    {
        masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
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