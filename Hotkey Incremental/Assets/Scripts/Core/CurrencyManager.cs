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
}
