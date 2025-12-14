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
    
    [Header("Update Settings")]
    [Tooltip("How often to update the main view display (in seconds). Lower = faster updates.")]
    public float updateInterval = 0.1f; // Default to 0.1 seconds (10 times per second)
    
    private void Start()
    {
        // Load update interval from PlayerPrefs if available
        if (PlayerPrefs.HasKey("MainViewUpdateInterval"))
        {
            updateInterval = PlayerPrefs.GetFloat("MainViewUpdateInterval", 0.1f);
        }
        
        StartCoroutine(RefreshMainLetterUI());
    }

    IEnumerator RefreshMainLetterUI()
    {
        while (true)
        {
            UpdateMainLetterDisplay();
            UpdateProductionDisplay();
            if (upgradeUI != null)
                upgradeUI.UpdateAllButtons();
            yield return new WaitForSeconds(updateInterval);
        }
    }
    
    // Method to change update interval (can be called from settings UI)
    public void SetUpdateInterval(float interval)
    {
        updateInterval = Mathf.Clamp(interval, 0.01f, 1.0f); // Clamp between 0.01 and 1 second
        PlayerPrefs.SetFloat("MainViewUpdateInterval", updateInterval);
        PlayerPrefs.Save();
    }

    void UpdateMainLetterDisplay()
    {
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
        if (productionRateText != null)
        {
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
    }
    
    // Method to switch to a specific letter view
    public void SwitchToLetter(string letter)
    {
        if (upgradeUI != null)
        {
            upgradeUI.SetCurrentLetter(letter);
        }
        
        if (letterPageController != null)
        {
            letterPageController.LoadLetter(letter);
        }
    }
    
    // Method to switch to a specific number view
    public void SwitchToNumber(int number)
    {
        if (numberView != null)
        {
            numberView.LoadNumber(number);
        }
    }
}
