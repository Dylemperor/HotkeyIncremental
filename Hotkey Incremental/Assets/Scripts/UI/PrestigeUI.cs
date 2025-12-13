using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class PrestigeUI : MonoBehaviour
{
    public PrestigeManager prestigeManager;
    public CurrencyManager currencyManager;
    
    [Header("Silver Prestige Button")]
    public Button silverPrestigeButton;
    public TMP_Text silverButtonText;
    public Image silverButtonImage;
    
    [Header("Gold Prestige Button")]
    public Button goldPrestigeButton;
    public TMP_Text goldButtonText;
    public Image goldButtonImage;
    
    [Header("Number Prestige Button")]
    public Button numberPrestigeButton;
    public TMP_Text numberButtonText;
    public Image numberButtonImage;
    public NumberPrestigeManager numberPrestigeManager;
    
    [Header("Update Settings")]
    public float updateInterval = 0.1f;
    
    // Silver and Gold colors with slight shine effect
    private Color silverColor = new Color(0.75f, 0.85f, 0.95f, 1f); // Slightly blue-tinted silver
    private Color goldColor = new Color(1f, 0.95f, 0.4f, 1f); // Rich gold color
    
    private void Start()
    {
        // Initially hide all buttons
        if (silverPrestigeButton != null)
            silverPrestigeButton.gameObject.SetActive(false);
        if (goldPrestigeButton != null)
            goldPrestigeButton.gameObject.SetActive(false);
        if (numberPrestigeButton != null)
            numberPrestigeButton.gameObject.SetActive(false);
        
        // Set up button colors
        SetupButtonColors();
        
        // Start update coroutine
        StartCoroutine(UpdatePrestigeUI());
    }
    
    private void SetupButtonColors()
    {
        // Setup silver button with shiny appearance
        if (silverButtonImage != null)
        {
            silverButtonImage.color = silverColor;
        }
        
        // Setup gold button with shiny appearance
        if (goldButtonImage != null)
        {
            goldButtonImage.color = goldColor;
        }
        
        // Setup button color blocks for hover/pressed states
        if (silverPrestigeButton != null)
        {
            var colors = silverPrestigeButton.colors;
            colors.normalColor = silverColor;
            colors.highlightedColor = new Color(silverColor.r * 1.2f, silverColor.g * 1.2f, silverColor.b * 1.2f, 1f);
            colors.pressedColor = new Color(silverColor.r * 0.8f, silverColor.g * 0.8f, silverColor.b * 0.8f, 1f);
            colors.selectedColor = silverColor;
            silverPrestigeButton.colors = colors;
        }
        
        if (goldPrestigeButton != null)
        {
            var colors = goldPrestigeButton.colors;
            colors.normalColor = goldColor;
            colors.highlightedColor = new Color(goldColor.r * 1.2f, goldColor.g * 1.2f, goldColor.b * 1.2f, 1f);
            colors.pressedColor = new Color(goldColor.r * 0.8f, goldColor.g * 0.8f, goldColor.b * 0.8f, 1f);
            colors.selectedColor = goldColor;
            goldPrestigeButton.colors = colors;
        }
    }
    
    private IEnumerator UpdatePrestigeUI()
    {
        while (true)
        {
            UpdateSilverButton();
            UpdateGoldButton();
            UpdateNumberButton();
            yield return new WaitForSeconds(updateInterval);
        }
    }
    
    private void UpdateSilverButton()
    {
        if (silverPrestigeButton == null || prestigeManager == null)
            return;
        
        bool canPrestige = prestigeManager.CanSilverPrestige();
        
        // Show button if silver prestige is available
        silverPrestigeButton.gameObject.SetActive(canPrestige);
        
        if (canPrestige && silverButtonText != null)
        {
            silverButtonText.text = "Silver Reset";
        }
    }
    
    private void UpdateGoldButton()
    {
        if (goldPrestigeButton == null || prestigeManager == null)
            return;
        
        bool canPrestige = prestigeManager.CanGoldPrestige();
        
        // Show button if gold prestige is available (and silver button exists)
        goldPrestigeButton.gameObject.SetActive(canPrestige);
        
        if (canPrestige && goldButtonText != null)
        {
            goldButtonText.text = "Gold Reset";
        }
    }
    
    private int GetTotalUnlockedLetters()
    {
        if (currencyManager == null)
            return 0;
        
        int count = 0;
        foreach (var pair in currencyManager.allLetters)
        {
            if (pair.Value.isUnlocked)
                count++;
        }
        return count;
    }
    
    // Button click handlers
    public void OnSilverPrestigeClick()
    {
        if (prestigeManager != null && prestigeManager.CanSilverPrestige())
        {
            prestigeManager.PerformSilverPrestige();
            prestigeManager.SavePrestigeData();
            
            // Notify other systems that prestige occurred
            // This might trigger UI updates in other components
            Debug.Log("Silver Prestige performed!");
        }
    }
    
    public void OnGoldPrestigeClick()
    {
        if (prestigeManager != null && prestigeManager.CanGoldPrestige())
        {
            prestigeManager.PerformGoldPrestige();
            prestigeManager.SavePrestigeData();
            
            // Notify other systems that prestige occurred
            Debug.Log("Gold Prestige performed!");
        }
    }
    
    private void UpdateNumberButton()
    {
        if (numberPrestigeButton == null || numberPrestigeManager == null)
            return;
        
        bool canPrestige = numberPrestigeManager.CanPerformNumberReset();
        
        // Show button if Number prestige is available
        numberPrestigeButton.gameObject.SetActive(canPrestige);
        
        if (canPrestige && numberButtonText != null)
        {
            numberButtonText.text = "Number Reset";
        }
    }
    
    public void OnNumberPrestigeClick()
    {
        if (numberPrestigeManager != null && numberPrestigeManager.CanPerformNumberReset())
        {
            numberPrestigeManager.PerformNumberReset();
            numberPrestigeManager.SaveNumberPrestigeData();
            
            // Also save prestige data since it resets gold plates
            if (prestigeManager != null)
            {
                prestigeManager.SavePrestigeData();
            }
            
            Debug.Log("Number Prestige performed!");
        }
    }
}

