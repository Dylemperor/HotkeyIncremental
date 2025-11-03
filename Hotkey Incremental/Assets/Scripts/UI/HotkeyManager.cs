using UnityEngine;
using UnityEngine.InputSystem;

public class HotkeyManager : MonoBehaviour
{
    public UpgradeUI upgradeUI;
    
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
            }
        }
    }
    
    private void MaxUpgradesForLetter(string letter)
    {
        if (upgradeUI != null)
        {
            upgradeUI.SetCurrentLetter(letter);
            upgradeUI.MaxAllUpgrades();
            Debug.Log($"Maxed upgrades for letter {letter}");
        }
        else
        {
            Debug.LogError($"HotkeyManager: Cannot max upgrades for {letter} - UpgradeUI is null!");
        }
    }
    
    public void MaxUpgradesForLetterPublic(string letter)
    {
        MaxUpgradesForLetter(letter);
    }
}
