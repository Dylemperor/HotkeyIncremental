using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CurrencyManager : MonoBehaviour
{
    public Dictionary<string, CurrencyData> allLetters = new Dictionary<string, CurrencyData>();

    private void Awake()
    {
        InitializeLetters();
    }

    void InitializeLetters()
    {
        for (char c = 'A'; c <= 'Z'; c++)
        {
            string letter = c.ToString();
            allLetters[letter] = new CurrencyData(letter);
        }
        
        // Unlock the first letter (A) by default
        allLetters["A"].isUnlocked = true;
    }

    public CurrencyData GetHighestUnlockedLetter()
    {
        return allLetters
            .Where(pair => pair.Value.isUnlocked)
            .OrderByDescending(pair => pair.Key)
            .FirstOrDefault().Value;
    }

    public void AddCurrency(string letter, double amount)
    {
        if (allLetters.ContainsKey(letter))
        {
            allLetters[letter].amount += amount;
        }
    }
    
    // Method to get all unlocked letters
    public List<CurrencyData> GetUnlockedLetters()
    {
        return allLetters.Values.Where(data => data.isUnlocked).ToList();
    }
    
    // Method to check if a letter is unlocked
    public bool IsLetterUnlocked(string letter)
    {
        return allLetters.ContainsKey(letter) && allLetters[letter].isUnlocked;
    }
    
    // Method to unlock the next letter
    public string UnlockNextLetter(string currentLetter)
    {
        if (string.IsNullOrEmpty(currentLetter) || currentLetter.Length != 1)
            return null;
            
        char currentChar = currentLetter[0];
        if (currentChar < 'A' || currentChar >= 'Z')
            return null; // Can't unlock beyond Z
            
        string nextLetter = ((char)(currentChar + 1)).ToString();
        
        if (allLetters.ContainsKey(nextLetter))
        {
            if (!allLetters[nextLetter].isUnlocked)
            {
                allLetters[nextLetter].isUnlocked = true;
                return nextLetter;
            }
        }
        
        return null;
    }
}
