

using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UnlockManager : MonoBehaviour
{
    public TMP_Text mainLetterDisplay;
    public CurrencyManager currencyManager;
    public LetterSelector letterSelector;

    public List<string> letterOrder = new List<string>()
    {
        "A","B","C","D","E","F","G","H","I","J","K","L","M","N","O","P","Q","R","S","T","U","V","W","X","Y","Z"
    };

    public double unlockThresholdMultiplier = 10000000000;

    private void Update()
    {
        CheckForUnlocks();
        UpdateMainLetterDisplay();
    }

    public void CheckForUnlocks()
    {
        for (int i = 1; i < letterOrder.Count; i++)
        {
            string prevLetter = letterOrder[i - 1];
            string currentLetter = letterOrder[i];

            if (!currencyManager.allLetters.ContainsKey(currentLetter)) continue;
            if (currencyManager.allLetters[currentLetter].isUnlocked) continue;

            // Don't unlock if the previous letter has 0 amount
            if (currencyManager.allLetters[prevLetter].amount <= 0) continue;

            double unlockCost = currencyManager.allLetters[prevLetter].amount * unlockThresholdMultiplier;

            if (currencyManager.allLetters[prevLetter].amount >= unlockCost)
            {
                UnlockLetter(currentLetter);
            }
        }
    }
    
    private void UnlockLetter(string letter)
    {
        currencyManager.allLetters[letter].isUnlocked = true;
        
        // Notify the letter selector to update the UI
        if (letterSelector != null)
        {
            letterSelector.OnLetterUnlocked(letter);
        }
        
        Debug.Log($"Unlocked letter: {letter}");
    }

    public CurrencyData GetHighestUnlockedLetter()
    {
        for (int i = letterOrder.Count - 1; i >= 0; i--)
        {
            string letterKey = letterOrder[i];
            if (currencyManager.allLetters.ContainsKey(letterKey) && currencyManager.allLetters[letterKey].isUnlocked)
            {
                return currencyManager.allLetters[letterKey];
            }
        }
        return null;
    }

    void UpdateMainLetterDisplay()
    {
        var highest = GetHighestUnlockedLetter();
        if (highest != null)
        {
            mainLetterDisplay.text = $"{highest.letter}: {NumberFormatter.Format(highest.amount)}";
        }
        else
        {
            mainLetterDisplay.text = "No letters unlocked yet!";
        }
    }
}