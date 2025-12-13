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
        while (true)
        {
            // Wait for the automation speed interval
            float waitTime = (float)numberManager.automationSpeed;
            yield return new WaitForSeconds(waitTime);
            
            // Purchase upgrades for each letter in the range
            foreach (string letter in letters)
            {
                if (currencyManager.allLetters.ContainsKey(letter))
                {
                    var letterData = currencyManager.allLetters[letter];
                    if (letterData.isUnlocked)
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

