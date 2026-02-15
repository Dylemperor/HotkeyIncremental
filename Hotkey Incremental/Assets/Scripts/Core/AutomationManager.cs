using UnityEngine;
using System.Collections;
using System.Linq;

public class AutomationManager : MonoBehaviour
{
    public NumberManager numberManager;
    public CurrencyManager currencyManager;
    public UpgradeUI upgradeUI;
    
    // First half: A-M (13 letters)
    private static readonly string[] FirstHalfLetters = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M" };
    
    // Second half: N-Z (13 letters)
    private static readonly string[] SecondHalfLetters = { "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
    
    private bool firstHalfEnabled = false;
    private bool secondHalfEnabled = false;
    
    private Coroutine firstHalfCoroutine;
    private Coroutine secondHalfCoroutine;
    
    private void Start()
    {
        // Validate required references
        if (numberManager == null)
        {
            Debug.LogError("AutomationManager: NumberManager is not assigned!");
            return;
        }
        if (currencyManager == null)
        {
            Debug.LogError("AutomationManager: CurrencyManager is not assigned!");
            return;
        }
        if (upgradeUI == null)
        {
            Debug.LogWarning("AutomationManager: UpgradeUI is not assigned. Automation may not work correctly.");
        }
        
        // Load automation states
        LoadAutomationData();
        
        // Start coroutines if enabled
        if (firstHalfEnabled)
        {
            StartFirstHalfAutomation();
        }
        if (secondHalfEnabled)
        {
            StartSecondHalfAutomation();
        }
    }
    
    public bool IsFirstHalfEnabled()
    {
        return firstHalfEnabled;
    }
    
    public bool IsSecondHalfEnabled()
    {
        return secondHalfEnabled;
    }
    
    public void SetFirstHalfEnabled(bool enabled)
    {
        if (numberManager == null)
        {
            Debug.LogWarning("NumberManager is null in AutomationManager!");
            return;
        }
        
        if (enabled && !numberManager.HasAutomationUpgrade(2))
        {
            Debug.LogWarning("First Half Automation upgrade not purchased!");
            return;
        }
        
        firstHalfEnabled = enabled;
        
        if (enabled)
        {
            StartFirstHalfAutomation();
        }
        else
        {
            StopFirstHalfAutomation();
        }
        
        SaveAutomationData();
    }
    
    public void SetSecondHalfEnabled(bool enabled)
    {
        if (numberManager == null)
        {
            Debug.LogWarning("NumberManager is null in AutomationManager!");
            return;
        }
        
        if (enabled && !numberManager.HasAutomationUpgrade(3))
        {
            Debug.LogWarning("Second Half Automation upgrade not purchased!");
            return;
        }
        
        secondHalfEnabled = enabled;
        
        if (enabled)
        {
            StartSecondHalfAutomation();
        }
        else
        {
            StopSecondHalfAutomation();
        }
        
        SaveAutomationData();
    }
    
    private void StartFirstHalfAutomation()
    {
        if (firstHalfCoroutine != null)
        {
            StopCoroutine(firstHalfCoroutine);
        }
        firstHalfCoroutine = StartCoroutine(AutomationLoop(FirstHalfLetters));
    }
    
    private void StopFirstHalfAutomation()
    {
        if (firstHalfCoroutine != null)
        {
            StopCoroutine(firstHalfCoroutine);
            firstHalfCoroutine = null;
        }
    }
    
    private void StartSecondHalfAutomation()
    {
        if (secondHalfCoroutine != null)
        {
            StopCoroutine(secondHalfCoroutine);
        }
        secondHalfCoroutine = StartCoroutine(AutomationLoop(SecondHalfLetters));
    }
    
    private void StopSecondHalfAutomation()
    {
        if (secondHalfCoroutine != null)
        {
            StopCoroutine(secondHalfCoroutine);
            secondHalfCoroutine = null;
        }
    }
    
    private IEnumerator AutomationLoop(string[] letters)
    {
        if (numberManager == null || currencyManager == null || upgradeUI == null)
        {
            Debug.LogError("AutomationManager: Cannot start automation loop - required managers are null");
            yield break;
        }
        
        if (letters == null || letters.Length == 0)
        {
            Debug.LogWarning("AutomationManager: No letters provided for automation loop");
            yield break;
        }
            
        while (true)
        {
            // Wait for the automation speed interval
            float waitTime = (float)numberManager.automationSpeed;
            
            // Validate wait time to prevent infinite loops or negative waits
            if (waitTime <= 0)
            {
                Debug.LogWarning($"AutomationManager: Invalid automation speed {waitTime}, using minimum 0.5s");
                waitTime = 0.5f;
            }
            
            yield return new WaitForSeconds(waitTime);
            
            // Purchase upgrades for each letter in the range
            foreach (string letter in letters)
            {
                if (string.IsNullOrEmpty(letter))
                {
                    Debug.LogWarning("AutomationManager: Encountered null or empty letter in automation loop");
                    continue;
                }
                
                if (currencyManager.allLetters == null)
                {
                    Debug.LogError("AutomationManager: CurrencyManager.allLetters is null");
                    yield break;
                }
                
                if (currencyManager.allLetters.ContainsKey(letter))
                {
                    var letterData = currencyManager.allLetters[letter];
                    if (letterData != null && letterData.isUnlocked)
                    {
                        // Use UpgradeUI's MaxAllUpgradesForLetter method
                        if (upgradeUI != null)
                        {
                            upgradeUI.MaxAllUpgradesForLetter(letter, false);
                        }
                    }
                }
            }
        }
    }
    
    public double GetAutomationSpeed()
    {
        if (numberManager != null)
        {
            return numberManager.automationSpeed;
        }
        return 30.0;
    }
    
    // Save/Load automation data
    public void SaveAutomationData()
    {
        PlayerPrefs.SetInt("Automation_FirstHalf", firstHalfEnabled ? 1 : 0);
        PlayerPrefs.SetInt("Automation_SecondHalf", secondHalfEnabled ? 1 : 0);
        PlayerPrefs.Save();
    }
    
    public void LoadAutomationData()
    {
        if (PlayerPrefs.HasKey("Automation_FirstHalf"))
        {
            firstHalfEnabled = PlayerPrefs.GetInt("Automation_FirstHalf", 0) == 1;
        }
        if (PlayerPrefs.HasKey("Automation_SecondHalf"))
        {
            secondHalfEnabled = PlayerPrefs.GetInt("Automation_SecondHalf", 0) == 1;
        }
    }
    
    public void ResetAutomationData()
    {
        firstHalfEnabled = false;
        secondHalfEnabled = false;
        StopFirstHalfAutomation();
        StopSecondHalfAutomation();
        
        PlayerPrefs.DeleteKey("Automation_FirstHalf");
        PlayerPrefs.DeleteKey("Automation_SecondHalf");
        PlayerPrefs.Save();
    }
}

