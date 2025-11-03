using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour
{
    [Header("Core Systems")]
    public CurrencyManager currencyManager;
    public ProductionManager productionManager;
    public UnlockManager unlockManager;
    public SaveManager saveManager;
    
    [Header("UI Systems")]
    public MainViewController mainViewController;
    public UpgradeUI upgradeUI;
    public LetterSelector letterSelector;
    
    [Header("Settings")]
    public float autoSaveInterval = 30f; // Auto save every 30 seconds
    
    private void Start()
    {
        // Load the game
        if (saveManager != null)
        {
            saveManager.LoadGame();
        }
        
        // Start auto-save coroutine
        StartCoroutine(AutoSave());
        
        Debug.Log("Game started successfully!");
    }
    
    private IEnumerator AutoSave()
    {
        while (true)
        {
            yield return new WaitForSeconds(autoSaveInterval);
            if (saveManager != null)
            {
                saveManager.SaveGame();
            }
        }
    }
    
    // Method to manually save the game
    public void SaveGame()
    {
        if (saveManager != null)
        {
            saveManager.SaveGame();
        }
    }
    
    // Method to reset the game
    public void ResetGame()
    {
        if (saveManager != null)
        {
            saveManager.ResetSave();
        }
        
        // Restart the scene or reload the game
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }
    
    // Method to get current game stats
    public GameStats GetGameStats()
    {
        var stats = new GameStats();
        
        if (currencyManager != null)
        {
            stats.totalLettersUnlocked = currencyManager.GetUnlockedLetters().Count;
            stats.highestLetter = currencyManager.GetHighestUnlockedLetter()?.letter ?? "None";
            
            double totalAmount = 0;
            foreach (var letter in currencyManager.allLetters.Values)
            {
                totalAmount += letter.amount;
            }
            stats.totalCurrency = totalAmount;
        }
        
        return stats;
    }
    
    // Method to add currency (for testing or special events)
    public void AddCurrencyToAll(double amount)
    {
        if (currencyManager != null)
        {
            foreach (var letter in currencyManager.GetUnlockedLetters())
            {
                currencyManager.AddCurrency(letter.letter, amount);
            }
        }
    }
    
    // Method to unlock a specific letter (for testing or special events)
    public void UnlockLetter(string letter)
    {
        if (currencyManager != null && currencyManager.allLetters.ContainsKey(letter))
        {
            currencyManager.allLetters[letter].isUnlocked = true;
            
            if (letterSelector != null)
            {
                letterSelector.OnLetterUnlocked(letter);
            }
        }
    }
}

// Class to hold game statistics
[System.Serializable]
public class GameStats
{
    public int totalLettersUnlocked;
    public string highestLetter;
    public double totalCurrency;
    
    public override string ToString()
    {
        return $"Letters Unlocked: {totalLettersUnlocked}, Highest: {highestLetter}, Total Currency: {NumberFormatter.Format(totalCurrency)}";
    }
} 