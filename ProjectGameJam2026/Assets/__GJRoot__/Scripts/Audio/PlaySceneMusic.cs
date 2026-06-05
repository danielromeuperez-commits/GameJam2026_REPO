using System.Collections;
using UnityEngine;

public class PlaySceneMusic : MonoBehaviour
{
    [Header("Music Index In AudioManager")]
    public int musicIndex = 1;

    private IEnumerator Start()
    {
        yield return null;

        if (AudioManager.Instance == null)
        {
            Debug.LogWarning("No AudioManager found.");
            yield break;
        }

        AudioManager.Instance.ResetMusicFadeMultiplier();
        AudioManager.Instance.RefreshAudioVolumes();
        AudioManager.Instance.PlayMusic(musicIndex);
    }
}