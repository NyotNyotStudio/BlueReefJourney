using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("UI Elements")]
    [SerializeField] private GameObject infoPanel;
    [SerializeField] private UnityEngine.UI.Text nameText;
    [SerializeField] private UnityEngine.UI.Text descriptionText;
    [SerializeField] private UnityEngine.UI.Image creatureImage;

    [Header("Progress UI")]
    [SerializeField] private TextMeshProUGUI scannedCountText;
    [SerializeField] private TextMeshProUGUI junkCountText;
    [SerializeField] private TextMeshProUGUI levelText;

    [Header("Level Progression Settings")]
    [SerializeField] private int baseFishTarget = 10;
    [SerializeField] private int baseJunkTarget = 5;
    [SerializeField] private int fishIncreasePerLevel = 5;
    [SerializeField] private int junkIncreasePerLevel = 3;
    [SerializeField] private int maxLevels = 4;
    [SerializeField] private string winSceneName = "YouWon";

    [Header("Sound Effects")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip closePanelSFX;
    [SerializeField] private AudioClip popUpPanelSFX;
    [SerializeField][Range(0f, 1f)] private float sfxVolume = 1f;

    private HashSet<string> scannedFish = new HashSet<string>();
    private HashSet<string> collectedJunk = new HashSet<string>();
    private bool isPanelOpen = false;

    private int currentLevel = 1;
    private int currentFishTarget;
    private int currentJunkTarget;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
    }

    private void Start()
    {
        CalculateTargets();

        infoPanel.SetActive(false);
        UpdateLevelUI();
        UpdateScannedText();
        UpdateJunkText();
    }

    private void CalculateTargets()
    {
        currentFishTarget = baseFishTarget + ((currentLevel - 1) * fishIncreasePerLevel);
        currentJunkTarget = baseJunkTarget + ((currentLevel - 1) * junkIncreasePerLevel);
    }

    private void Update()
    {
        if (isPanelOpen && Input.GetKeyDown(KeyCode.Q))
        {
            PlaySound(closePanelSFX);
            HideCreatureInfo();
        }
    }

    public bool IsJunkComplete()
    {
        return collectedJunk.Count >= currentJunkTarget;
    }

    public bool IsFishScanned(string creatureName)
    {
        return scannedFish.Contains(creatureName);
    }

    public void ShowCreatureInfo(FishInfo creature)
    {
        if (creature == null) return;

        if (scannedFish.Contains(creature.creatureName)) return;

        nameText.text = creature.creatureName;
        descriptionText.text = creature.description;
        creatureImage.sprite = creature.image;

        infoPanel.SetActive(true);
        isPanelOpen = true;

        PlaySound(popUpPanelSFX);

        scannedFish.Add(creature.creatureName);
        UpdateScannedText();
        CheckWinCondition();
    }

    public void CollectJunk(string junkName)
    {
        if (!collectedJunk.Contains(junkName))
        {
            collectedJunk.Add(junkName);
            UpdateJunkText();
            CheckWinCondition();
        }
    }

    public void HideCreatureInfo()
    {
        infoPanel.SetActive(false);
        isPanelOpen = false;
    }

    private void UpdateScannedText()
    {
        if (scannedCountText != null)
        {
            scannedCountText.text = $"{scannedFish.Count}/{currentFishTarget}";
        }
    }

    private void UpdateJunkText()
    {
        if (junkCountText != null)
        {
            junkCountText.text = $"{collectedJunk.Count}/{currentJunkTarget}";
        }
    }

    private void UpdateLevelUI()
    {
        if (levelText != null)
        {
            levelText.text = $"Level {currentLevel}";
        }
    }

    private void CheckWinCondition()
    {
        bool allFishFound = scannedFish.Count >= currentFishTarget;
        bool allJunkFound = collectedJunk.Count >= currentJunkTarget;

        if (allFishFound && allJunkFound)
        {
            if (currentLevel < maxLevels)
            {
                currentLevel++;
                PlayerPrefs.SetInt("CurrentLevel", currentLevel);
                PlayerPrefs.Save();
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
            else
            {
                PlayerPrefs.SetInt("CurrentLevel", 1);
                PlayerPrefs.Save();
                SceneManager.LoadScene(winSceneName);
            }
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip, sfxVolume);
        }
    }
}