using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Healthbar : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float baseBoostDrainSpeed = 30f;
    [SerializeField] private string gameOverScene = "GameOver";
    [SerializeField] private TextMeshProUGUI healthText;

    private float currentHealth;
    private float currentDrainSpeed;
    private bool isDead = false;

    private void Start()
    {
        int currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);

        float difficultyMultiplier = 1f + ((currentLevel - 1) * 0.25f);
        currentDrainSpeed = baseBoostDrainSpeed * difficultyMultiplier;

        currentHealth = maxHealth;
        UpdateUI();
        isDead = false;
    }

    private void Update()
    {
        if (isDead) return;

        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            currentHealth -= currentDrainSpeed * Time.deltaTime;
        }

        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateUI();

        if (currentHealth <= 0 && !isDead)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        if (isDead) return;

        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (healthText != null)
        {
            if (currentHealth <= 0)
            {
                healthText.text = "0%";
            }
            else
            {
                healthText.text = Mathf.CeilToInt(currentHealth).ToString() + "%";
            }
        }
    }

    private void Die()
    {
        isDead = true;

        if (healthText != null) healthText.text = "0%";

        Invoke("TriggerGameOver", 0.5f);
    }

    private void TriggerGameOver()
    {
        PlayerPrefs.SetInt("CurrentLevel", 1);
        PlayerPrefs.Save();
        SceneManager.LoadScene(gameOverScene);
    }
}