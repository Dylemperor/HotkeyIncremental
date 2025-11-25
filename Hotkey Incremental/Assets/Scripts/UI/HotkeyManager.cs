using UnityEngine;
using UnityEngine.InputSystem;

public class HotkeyManager : MonoBehaviour
{
    public UpgradeUI upgradeUI;
    public CurrencyManager currencyManager;
    public LetterSelector letterSelector;
    
    private const double UNLOCK_CURRENCY_THRESHOLD = 1000000000; // 1 billion
    
    private void Start()
    {
        if (upgradeUI == null)
        {
            Debug.LogError("HotkeyManager: UpgradeUI reference is missing!");
        }
        else
        {
            Debug.Log("HotkeyManager: UpgradeUI reference found and ready for hotkeys A-Z");
        }
    }
    
    private void Update()
    {
        HandleHotkeys();
    }
    
    private void HandleHotkeys()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return; // No keyboard available
        
        for (int i = 0; i < 26; i++)
        {
            Key key = Key.A + i; // A..Z
            if (keyboard[key].wasPressedThisFrame)
            {
                string letter = ((char)('A' + i)).ToString();
                Debug.Log($"Hotkey pressed: {letter}");
                MaxUpgradesForLetter(letter);
                TryUnlockNextLetter(letter);
            }
        }
    }
    
    private void MaxUpgradesForLetter(string letter)
    {
        if (upgradeUI != null)
        {
            // Buy upgrades for the letter without switching the UI
            upgradeUI.MaxAllUpgradesForLetter(letter, false);
            Debug.Log($"Maxed upgrades for letter {letter}");
        }
        else
        {
            Debug.LogError($"HotkeyManager: Cannot max upgrades for {letter} - UpgradeUI is null!");
        }
    }
    
    private void TryUnlockNextLetter(string letter)
    {
        if (currencyManager == null)
            return;
        
        // Check if the letter is unlocked
        if (!currencyManager.IsLetterUnlocked(letter))
            return;
        
        // Check if we can unlock the next letter
        if (!currencyManager.allLetters.ContainsKey(letter))
            return;
        
        var data = currencyManager.allLetters[letter];
        
        // Check if current letter has at least 1 billion
        if (data.amount < UNLOCK_CURRENCY_THRESHOLD)
            return;
        
        // Check if next letter exists and isn't already unlocked
        string nextLetter = GetNextLetter(letter);
        if (nextLetter == null)
            return; // Can't unlock beyond Z
        
        if (currencyManager.IsLetterUnlocked(nextLetter))
            return; // Already unlocked
        
        // Try to unlock the next letter
        string unlockedLetter = currencyManager.UnlockNextLetter(letter);
        if (unlockedLetter != null)
        {
            // Notify LetterSelector to update UI
            if (letterSelector != null)
            {
                letterSelector.OnLetterUnlocked(unlockedLetter);
            }
            
            Debug.Log($"Hotkey unlocked next letter: {unlockedLetter}");
        }
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
    
    public void MaxUpgradesForLetterPublic(string letter)
    {
        MaxUpgradesForLetter(letter);
        TryUnlockNextLetter(letter);
    }
}
