using TMPro;
using UnityEngine;
using System.Collections;

public class MainViewController : MonoBehaviour
{
    public TMP_Text mainLetterDisplayText;
    public TMP_Text productionRateText;
    public CurrencyManager currencyManager;
    public ProductionManager productionManager;
    public UpgradeUI upgradeUI;
    public LetterPageController letterPageController;
    public NumberView numberView;
    public NumberManager numberManager;
    
    [Header("UI Elements")]
    public GameObject automationButton; // Button to navigate to automation page
    
    [Header("Update Settings")]
    [Tooltip("How often to update the main view display (in seconds). Lower = faster updates.")]
    public float updateInterval = 0.1f; // Default to 0.1 seconds (10 times per second)
    
    private void Start()
    {
        // Validate required references
        if (currencyManager == null)
        {
            Debug.LogError("MainViewController: CurrencyManager is not assigned!");
        }
        if (productionManager == null)
        {
            Debug.LogError("MainViewController: ProductionManager is not assigned!");
        }
        
        // Load update interval from PlayerPrefs if available
        if (PlayerPrefs.HasKey("MainViewUpdateInterval"))
        {
            updateInterval = PlayerPrefs.GetFloat("MainViewUpdateInterval", 0.1f);
        }
        
        // Validate update interval
        if (updateInterval <= 0)
        {
            Debug.LogWarning("MainViewController: Invalid update interval, resetting to default 0.1s");
            updateInterval = 0.1f;
        }
        
        // Set initial automation button visibility
        UpdateAutomationButtonVisibility();
        
        StartCoroutine(RefreshMainLetterUI());
    }

    IEnumerator RefreshMainLetterUI()
    {
        while (true)
        {
            UpdateMainLetterDisplay();
            UpdateProductionDisplay();
            UpdateAutomationButtonVisibility();
            if (upgradeUI != null)
                upgradeUI.UpdateAllButtons();
            yield return new WaitForSeconds(updateInterval);
        }
    }
    
    // Method to change update interval (can be called from settings UI)
    public void SetUpdateInterval(float interval)
    {
        if (interval <= 0)
        {
            Debug.LogWarning($"MainViewController: Invalid update interval {interval}, using minimum 0.01s");
            interval = 0.01f;
        }
        
        updateInterval = Mathf.Clamp(interval, 0.01f, 1.0f); // Clamp between 0.01 and 1 second
        PlayerPrefs.SetFloat("MainViewUpdateInterval", updateInterval);
        PlayerPrefs.Save();
        
        Debug.Log($"MainViewController: Update interval set to {updateInterval}s");
    }

    void UpdateMainLetterDisplay()
    {
        if (mainLetterDisplayText == null || currencyManager == null)
            return;
            
        var highest = currencyManager.GetHighestUnlockedLetter();
        if (highest != null)
        {
            mainLetterDisplayText.text = $"{highest.letter}: {NumberFormatter.Format(highest.amount)}";
        }
        else
        {
            mainLetterDisplayText.text = "No letters unlocked yet!";
        }
    }
    
    void UpdateProductionDisplay()
    {
        if (productionRateText == null || currencyManager == null || productionManager == null)
            return;
            
        var highest = currencyManager.GetHighestUnlockedLetter();
        if (highest != null)
        {
            double productionRate = productionManager.GetProductionRate(highest.letter);
            productionRateText.text = $"Production: {NumberFormatter.Format(productionRate)}/s";
        }
        else
        {
            productionRateText.text = "Production: 0/s";
        }
    }
    
    void UpdateAutomationButtonVisibility()
    {
        if (automationButton != null && numberManager != null)
        {
            // Show automation button if either First Half or Second Half automation is unlocked
            bool hasAutomation = numberManager.HasAutomationUpgrade(2) || numberManager.HasAutomationUpgrade(3);
            automationButton.SetActive(hasAutomation);
        }
    }
    
    // Method to switch to a specific letter view
    public void SwitchToLetter(string letter)
    {
        if (string.IsNullOrEmpty(letter))
        {
            Debug.LogWarning("MainViewController: Cannot switch to letter - letter is null or empty");
            return;
        }
        
        if (letter.Length != 1 || !char.IsLetter(letter[0]))
        {
            Debug.LogWarning($"MainViewController: Invalid letter '{letter}' - must be a single letter");
            return;
        }
        
        if (upgradeUI != null)
        {
            upgradeUI.SetCurrentLetter(letter);
        }
        else
        {
            Debug.LogWarning("MainViewController: UpgradeUI is null, cannot set current letter");
        }
        
        if (letterPageController != null)
        {
            letterPageController.LoadLetter(letter);
        }
        else
        {
            Debug.LogWarning("MainViewController: LetterPageController is null, cannot load letter");
        }
    }
    
    // Method to switch to a specific number view
    public void SwitchToNumber(int number)
    {
        if (number < 1 || number > 9)
        {
            Debug.LogWarning($"MainViewController: Invalid number {number} - must be between 1 and 9");
            return;
        }
        
        if (numberView != null)
        {
            numberView.LoadNumber(number);
        }
        else
        {
            Debug.LogWarning("MainViewController: NumberView is null, cannot load number");
        }
    }
}
