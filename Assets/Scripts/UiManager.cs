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
    [SerializeField] private int totalFishTypes = 10;
    [SerializeField] private int totalJunkItem = 5;
    [SerializeField] private string winSceneName = "YouWon";

    [Header("Sound Effects")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip closePanelSFX;
    [SerializeField] private AudioClip popUpPanelSFX;
    [SerializeField][Range(0f, 1f)] private float sfxVolume = 1f;

    private HashSet<string> scannedFish = new HashSet<string>();
    private HashSet<string> collectedJunk = new HashSet<string>();
    private bool isPanelOpen = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        infoPanel.SetActive(false);
        UpdateScannedText();
        UpdateJunkText();
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
        return collectedJunk.Count >= totalJunkItem;
    }

    public bool IsFishScanned(string creatureName)
    {
        return scannedFish.Contains(creatureName);
    }

    public void ShowCreatureInfo(FishInfo creature)
    {
        if (creature == null) return;

        nameText.text = creature.creatureName;
        descriptionText.text = creature.description;
        creatureImage.sprite = creature.image;

        infoPanel.SetActive(true);
        isPanelOpen = true;

        PlaySound(popUpPanelSFX);

        if (!scannedFish.Contains(creature.creatureName))
        {
            scannedFish.Add(creature.creatureName);
            UpdateScannedText();
            CheckWinCondition();
        }
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
            scannedCountText.text = $"{scannedFish.Count}/{totalFishTypes}";
        }
    }

    private void UpdateJunkText()
    {
        if (junkCountText != null)
        {
            junkCountText.text = $"{collectedJunk.Count}/{totalJunkItem}";
        }
    }

    private void CheckWinCondition()
    {
        bool allFishFound = scannedFish.Count >= totalFishTypes;
        bool allJunkFound = collectedJunk.Count >= totalJunkItem;

        if (allFishFound && allJunkFound)
        {
            SceneManager.LoadScene(winSceneName);
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