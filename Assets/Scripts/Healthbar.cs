using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float boostDrainSpeed = 30f;
    [SerializeField] private string gameOverScene = "GameOver";
    [SerializeField] private TextMeshProUGUI healthText;

    private float currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateUI();
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            currentHealth -= boostDrainSpeed * Time.deltaTime;
        }

        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateUI();

        if (currentHealth <= 0)
        {
            SceneManager.LoadScene(gameOverScene);
        }
    }

    private void UpdateUI()
    {
        if (healthText != null)
        {
            healthText.text = Mathf.CeilToInt(currentHealth).ToString() + "%";
        }
    }
}