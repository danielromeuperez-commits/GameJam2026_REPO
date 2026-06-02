using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] float PlHealth = 100;
    [SerializeField] float PlCurrHealth;

    private void Awake()
    {
        PlHealth = PlCurrHealth;
    }

}
