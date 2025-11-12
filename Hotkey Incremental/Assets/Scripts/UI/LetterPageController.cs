using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class LetterPageController : MonoBehaviour
{
    public TMP_Text letterAmountText;
    public TMP_Text letterProductionText;
    public CurrencyManager currencyManager;
    public ProductionManager productionManager;
    public LetterSelector letterSelector;
    public Button unlockNextLetterButton;
    public TMP_Text unlockButtonText;
    public string currentLetter = "A";
    
    private const double UNLOCK_CURRENCY_THRESHOLD = 1000000000; // 1 billion
    
    private void Start()
    {
        StartCoroutine(UpdateLetterDisplay());
    }
    
    IEnumerator UpdateLetterDisplay()
    {
        while (true)
        {
            UpdateLetterInfo();
            yield return new WaitForSeconds(0.5f);
        }
    }
    
    public void LoadLetter(string letter)
    {
        currentLetter = letter;
        UpdateLetterInfo();
    }
    
    private void UpdateLetterInfo()
    {
        if (currencyManager.allLetters.ContainsKey(currentLetter))
        {
            var data = currencyManager.allLetters[currentLetter];
            
            // Update amount text
            if (letterAmountText != null)
            {
                letterAmountText.text = $"{currentLetter}: {NumberFormatter.Format(data.amount)}";
            }
            
            // Update production text
            if (letterProductionText != null)
            {
                double productionRate = productionManager.GetProductionRate(currentLetter);
                letterProductionText.text = $"Production: {NumberFormatter.Format(productionRate)}/s";
            }
            
            // Update unlock button state
            UpdateUnlockButton();
        }
    }
    
    private void UpdateUnlockButton()
    {
        if (unlockNextLetterButton == null)
            return;
            
        bool canUnlock = CanUnlockNextLetter();
        unlockNextLetterButton.interactable = canUnlock;
        
        // Update button text
        if (unlockButtonText != null)
        {
            if (canUnlock)
            {
                string nextLetter = GetNextLetter(currentLetter);
                if (nextLetter != null)
                {
                    unlockButtonText.text = $"Unlock {nextLetter}";
                }
            }
            else
            {
                unlockButtonText.text = "Unlock Next Letter Cost: " + NumberFormatter.Format(UNLOCK_CURRENCY_THRESHOLD);
            }
        }
        
        // Update button visual state
        var colors = unlockNextLetterButton.colors;
        if (canUnlock)
        {
            colors.normalColor = Color.white;
            colors.selectedColor = Color.green;
        }
        else
        {
            colors.normalColor = Color.gray;
            colors.selectedColor = Color.gray;
        }
        unlockNextLetterButton.colors = colors;
    }
    
    private bool CanUnlockNextLetter()
    {
        if (!currencyManager.allLetters.ContainsKey(currentLetter))
            return false;
            
        var data = currencyManager.allLetters[currentLetter];
        
        // Check if current letter has at least 1 billion
        if (data.amount < UNLOCK_CURRENCY_THRESHOLD)
            return false;
            
        // Check if next letter exists and isn't already unlocked
        string nextLetter = GetNextLetter(currentLetter);
        if (nextLetter == null)
            return false;
            
        if (currencyManager.IsLetterUnlocked(nextLetter))
            return false;
            
        return true;
    }
    
    private string GetNextLetter(string currentLetter)
    {
        if (string.IsNullOrEmpty(currentLetter) || currentLetter.Length != 1)
            return null;
            
        char currentChar = currentLetter[0];
        if (currentChar < 'A' || currentChar >= 'Z')
            return null; // Can't unlock beyond Z
            
        return ((char)(currentChar + 1)).ToString();
    }
    
    public void UnlockNextLetter()
    {
        if (!CanUnlockNextLetter())
            return;
            
        string nextLetter = currencyManager.UnlockNextLetter(currentLetter);
        if (nextLetter != null)
        {
            // Notify LetterSelector to update UI
            if (letterSelector != null)
            {
                letterSelector.OnLetterUnlocked(nextLetter);
            }
            
            Debug.Log($"Unlocked letter: {nextLetter}");
            UpdateUnlockButton();
        }
    }
}
