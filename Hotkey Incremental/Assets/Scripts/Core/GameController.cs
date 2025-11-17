using UnityEngine;
using System.Collections;
using System.Reflection;

public class GameController : MonoBehaviour
{
    [Header("Core Systems")]
    public CurrencyManager currencyManager;
    public ProductionManager productionManager;
    public UnlockManager unlockManager;
    public SaveManager saveManager;
    
    // WebSaveManager for web builds - assign WebSaveManager component in inspector
    public MonoBehaviour webSaveManager;
    
    [Header("UI Systems")]
    public MainViewController mainViewController;
    public UpgradeUI upgradeUI;
    public LetterSelector letterSelector;
    
    [Header("Settings")]
    public float autoSaveInterval = 90f; // Auto save every 2 minutes (120 seconds)
    
    private void Start()
    {
        // Wait a frame to ensure CurrencyManager is initialized
        StartCoroutine(InitializeGame());
    }
    
    private IEnumerator InitializeGame()
    {
        // Wait for CurrencyManager to be ready
        yield return null;
        
        // Load the game - prefer WebSaveManager for web builds
        #if UNITY_WEBGL && !UNITY_EDITOR
        if (webSaveManager != null)
        {
            var method = webSaveManager.GetType().GetMethod("LoadGame");
            if (method != null)
            {
                method.Invoke(webSaveManager, null);
            }
        }
        else if (saveManager != null)
        {
            saveManager.LoadGame();
        }
        #else
        if (saveManager != null)
        {
            saveManager.LoadGame();
        }
        else if (webSaveManager != null)
        {
            var method = webSaveManager.GetType().GetMethod("LoadGame");
            if (method != null)
            {
                method.Invoke(webSaveManager, null);
            }
        }
        #endif
        
        // Start auto-save coroutine
        StartCoroutine(AutoSave());
        
        Debug.Log("Game started successfully!");
    }
    
    private void OnApplicationFocus(bool hasFocus)
    {
        // Save when the application loses focus (user switches tabs/windows)
        // This is important for web builds where users might close the tab
        if (!hasFocus)
        {
            SaveGameNow();
            Debug.Log("Game saved on focus loss");
        }
    }
    
    private void OnApplicationPause(bool pauseStatus)
    {
        // Save when the application is paused
        // This works for mobile builds and some web scenarios
        if (pauseStatus)
        {
            SaveGameNow();
            Debug.Log("Game saved on pause");
        }
    }
    
    private void OnApplicationQuit()
    {
        // Save when the application is quitting
        // Note: This may not always fire in web builds, so OnApplicationFocus is important
        SaveGameNow();
        Debug.Log("Game saved on quit");
    }
    
    private void SaveGameNow()
    {
        #if UNITY_WEBGL && !UNITY_EDITOR
        if (webSaveManager != null)
        {
            // Use reflection to call SaveGame on WebSaveManager
            var method = webSaveManager.GetType().GetMethod("SaveGame");
            if (method != null)
            {
                method.Invoke(webSaveManager, null);
            }
        }
        else if (saveManager != null)
        {
            saveManager.SaveGame();
        }
        #else
        if (saveManager != null)
        {
            saveManager.SaveGame();
        }
        else if (webSaveManager != null)
        {
            var method = webSaveManager.GetType().GetMethod("SaveGame");
            if (method != null)
            {
                method.Invoke(webSaveManager, null);
            }
        }
        #endif
    }
    
    private IEnumerator AutoSave()
    {
        while (true)
        {
            yield return new WaitForSeconds(autoSaveInterval);
            SaveGameNow();
            Debug.Log($"Auto-saved game (every {autoSaveInterval} seconds)");
        }
    }
    
    // Method to manually save the game
    public void SaveGame()
    {
        SaveGameNow();
    }
    
    // Method to reset the game
    public void ResetGame()
    {
        #if UNITY_WEBGL && !UNITY_EDITOR
        if (webSaveManager != null)
        {
            var method = webSaveManager.GetType().GetMethod("ResetSave");
            if (method != null)
            {
                method.Invoke(webSaveManager, null);
            }
        }
        else if (saveManager != null)
        {
            saveManager.ResetSave();
        }
        #else
        if (saveManager != null)
        {
            saveManager.ResetSave();
        }
        else if (webSaveManager != null)
        {
            var method = webSaveManager.GetType().GetMethod("ResetSave");
            if (method != null)
            {
                method.Invoke(webSaveManager, null);
            }
        }
        #endif
        
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