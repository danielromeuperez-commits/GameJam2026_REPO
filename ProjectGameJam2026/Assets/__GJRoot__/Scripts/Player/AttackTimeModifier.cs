using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AttackTimeModifier : MonoBehaviour
{
    [Header("Base settings")]
    public float baseAttackTime = 5f;

    [Header("Warning UI")]
    public CanvasGroup warningCanvas;   // Canvas con alpha control
    public float flashDuration = 0.2f;
    public int flashCount = 3;

    private float resultTime = 5f;

    public IEnumerator PrepareAttack(System.Action<float> onReady)
    {
        float r = Random.value;
        Debug.Log(r);
        if(r <= 0.15f) 
        {
            resultTime = 2f;

            if (warningCanvas != null)
            {
                yield return StartCoroutine(FlashWarning());
            }
        }
        else if(r <= 0.25f) 
        {
            resultTime = 7f;

            if (warningCanvas != null)
            {
                yield return StartCoroutine(FlashWarning());
            }
        }
        else 
        {
            resultTime = baseAttackTime; //10
        }

        //devolver resultado
        onReady?.Invoke(resultTime);
    }

    private IEnumerator FlashWarning()
    {
        for (int i = 0; i < flashCount; i++)
        {
            SetAlpha(1f);
            yield return new WaitForSeconds(flashDuration);

            SetAlpha(0f);
            yield return new WaitForSeconds(flashDuration);
        }

        SetAlpha(0f);
    }

    private void SetAlpha(float a)
    {
        warningCanvas.alpha = a;
        warningCanvas.gameObject.SetActive(a > 0f);
    }
}