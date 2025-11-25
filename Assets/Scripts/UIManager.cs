using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject infoPanel;
    public Text nameText;
    public Text descriptionText;
    public Image creatureImage;

    [Header("Progress UI")]
    public TextMeshProUGUI scannedCountText; // UI text for scanned count
    public int totalFishTypes = 10;          // set in Inspector (how many types exist)
    private HashSet<string> scannedFish = new HashSet<string>(); // store unique scanned fish

    [Header("Sound Effects")]
    public AudioSource audioSource;
    public AudioClip closePanelSFX;
    public AudioClip popUpPanelSFX;
    public float sfxVolume = 1f;

    private bool isPanelOpen = false;

    void Start()
    {
        infoPanel.SetActive(false); // hidden saat awal
    }

    void Update()
    {
        // Tekan Q untuk nutup pop-up
        if (isPanelOpen && Input.GetKeyDown(KeyCode.Q))
        {
            // Play close sound
            if (closePanelSFX != null)
            {
                if (audioSource != null)
                {
                    audioSource.PlayOneShot(closePanelSFX, sfxVolume);
                }
            }
            HideCreatureInfo();
            isPanelOpen = true;
        }
    }

    public void ShowCreatureInfo(HewanLaut creature)
    {
        infoPanel.SetActive(true);
        // Play Pop Up Sound Effect
        if (popUpPanelSFX != null)
        {
            if (audioSource != null)
            {
                audioSource.PlayOneShot(popUpPanelSFX, sfxVolume);
            }
        }
        nameText.text = creature.creatureName;
        descriptionText.text = creature.description;
        creatureImage.sprite = creature.image;
        isPanelOpen = true;

        // Add fish to scanned list
        if (!scannedFish.Contains(creature.creatureName))
        {
            scannedFish.Add(creature.creatureName);
            UpdateScannedText();

            //Check apakah semua sudah discan
            if (scannedFish.Count >= totalFishTypes)
            {
                SceneManager.LoadScene("YouWon");
            }
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
}
