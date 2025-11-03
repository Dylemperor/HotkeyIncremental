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
    
    private void Start()
    {
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
            yield return new WaitForSeconds(0.5f);
        }
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
}
