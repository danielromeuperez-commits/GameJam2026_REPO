using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    public AudioMixer mixer;
    public Slider slider;

    private void Start()
    {
        // Cargar valor guardado
        float savedVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);

        slider.value = savedVolume;
        SetVolume(savedVolume);

        slider.onValueChanged.AddListener(SetVolume);
    }

    public void SetVolume(float value)
    {
        float volume = Mathf.Log10(value) * 20;
        mixer.SetFloat("MasterVolume", volume);

        // Guardar valor
        PlayerPrefs.SetFloat("MasterVolume", value);
    }
}