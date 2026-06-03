using UnityEngine;

public class SimpleFloatingUI : MonoBehaviour

{
    [Header("Floating Settings")]
    public float floatHeight = 6f;
    public float floatSpeed = 2f;

    private Vector3 startPosition;

    private void OnEnable()
    {
        startPosition = transform.localPosition;
    }

    private void Update()
    {
        float yOffset = Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.localPosition = startPosition + new Vector3(0f, yOffset, 0f);
    }

}