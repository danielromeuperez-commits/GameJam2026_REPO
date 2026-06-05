using System.Collections;
using UnityEngine;

public class PlaySceneMusic : MonoBehaviour
{
    [Header("Music Index In AudioManager")]
    public int musicIndex = 0;

    private IEnumerator Start()
    {
        yield return null;

        if (AudioManager.Instance == null)
        {
            Debug.LogWarning("No AudioManager found. Start from Main Menu.");
            yield break;
        }

        AudioManager.Instance.ApplyVolumes();
        AudioManager.Instance.PlayMusic(musicIndex);
    }
}