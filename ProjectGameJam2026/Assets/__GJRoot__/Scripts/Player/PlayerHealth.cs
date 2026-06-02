using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("UI")]
    public Image healthFill;

    [Header("Scene")]
    [SerializeField] string sceneName;

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth < 0)
            currentHealth = 0;

        UpdateHealthBar();

        Debug.Log($"{gameObject.name} HP: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void UpdateHealthBar()
    {
        if (healthFill != null)
        {
            healthFill.fillAmount =
                (float)currentHealth / maxHealth;
        }
    }
    private void Die()
    {
        Debug.Log($"{gameObject.name} has been defeated");
        if (currentHealth <= 0)
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}