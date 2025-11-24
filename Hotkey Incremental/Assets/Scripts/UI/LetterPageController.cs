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
    
    [Header("Update Settings")]
    [Tooltip("How often to update the letter display (in seconds). Lower = faster updates.")]
    public float updateInterval = 0.1f; // Default to 0.1 seconds (10 times per second)
    
    private const double UNLOCK_CURRENCY_THRESHOLD = 1000000000; // 1 billion
    
    private void Start()
    {
        // Load update interval from PlayerPrefs if available
        if (PlayerPrefs.HasKey("LetterViewUpdateInterval"))
        {
            updateInterval = PlayerPrefs.GetFloat("LetterViewUpdateInterval", 0.1f);
        }
        
        StartCoroutine(UpdateLetterDisplay());
    }
    
    IEnumerator UpdateLetterDisplay()
    {
        while (true)
        {
            UpdateLetterInfo();
            yield return new WaitForSeconds(updateInterval);
        }
    }
    
    // Method to change update interval (can be called from settings UI)
    public void SetUpdateInterval(float interval)
    {
        updateInterval = Mathf.Clamp(interval, 0.01f, 1.0f); // Clamp between 0.01 and 1 second
        PlayerPrefs.SetFloat("LetterViewUpdateInterval", updateInterval);
        PlayerPrefs.Save();
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
            colors.highlightedColor = Color.gray;
            colors.pressedColor = Color.gray;
            colors.selectedColor = Color.gray;
        }
        else
        {
            colors.normalColor = Color.gray;
            colors.highlightedColor = Color.gray;
            colors.pressedColor = Color.gray;
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
