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
    public HealthBarJuice healthBarJuice;

    [Header("Scene")]
    [SerializeField] string sceneName;

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBarInstant();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth < 0)
            currentHealth = 0;

        UpdateHealthBarWithFeedback();

        Debug.Log($"{gameObject.name} HP: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void UpdateHealthBarInstant()
    {
        float health01 = (float)currentHealth / maxHealth;

        if (healthFill != null)
            healthFill.fillAmount = health01;

        if (healthBarJuice != null)
            healthBarJuice.SetHealth01Instant(health01);
    }

    private void UpdateHealthBarWithFeedback()
    {
        float health01 = (float)currentHealth / maxHealth;

        if (healthBarJuice != null)
        {
            healthBarJuice.SetHealth01(health01);
        }
        else if (healthFill != null)
        {
            healthFill.fillAmount = health01;
        }
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} has been defeated");

        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}