using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

public class LetterSelector : MonoBehaviour
{
    public CurrencyManager currencyManager;
    public MainViewController mainViewController;
    public Button[] letterButtons; // Assign letter buttons in inspector (A-Z)
    public TMP_Text[] letterTexts; // Optional: assign text components to show letter names
    
    private void Start()
    {
        UpdateAllLetterButtons();
    }
    
    // Functions to attach to buttons in the inspector
    public void SelectLetterA() { SelectLetter("A"); }
    public void SelectLetterB() { SelectLetter("B"); }
    public void SelectLetterC() { SelectLetter("C"); }
    public void SelectLetterD() { SelectLetter("D"); }
    public void SelectLetterE() { SelectLetter("E"); }
    public void SelectLetterF() { SelectLetter("F"); }
    public void SelectLetterG() { SelectLetter("G"); }
    public void SelectLetterH() { SelectLetter("H"); }
    public void SelectLetterI() { SelectLetter("I"); }
    public void SelectLetterJ() { SelectLetter("J"); }
    public void SelectLetterK() { SelectLetter("K"); }
    public void SelectLetterL() { SelectLetter("L"); }
    public void SelectLetterM() { SelectLetter("M"); }
    public void SelectLetterN() { SelectLetter("N"); }
    public void SelectLetterO() { SelectLetter("O"); }
    public void SelectLetterP() { SelectLetter("P"); }
    public void SelectLetterQ() { SelectLetter("Q"); }
    public void SelectLetterR() { SelectLetter("R"); }
    public void SelectLetterS() { SelectLetter("S"); }
    public void SelectLetterT() { SelectLetter("T"); }
    public void SelectLetterU() { SelectLetter("U"); }
    public void SelectLetterV() { SelectLetter("V"); }
    public void SelectLetterW() { SelectLetter("W"); }
    public void SelectLetterX() { SelectLetter("X"); }
    public void SelectLetterY() { SelectLetter("Y"); }
    public void SelectLetterZ() { SelectLetter("Z"); }
    
    private void SelectLetter(string letter)
    {
        if (currencyManager.IsLetterUnlocked(letter))
        {
            if (mainViewController != null)
            {
                mainViewController.SwitchToLetter(letter);
            }
            Debug.Log($"Selected letter: {letter}");
        }
    }
    
    // Method to update all letter buttons (call when letters are unlocked)
    public void UpdateAllLetterButtons()
    {
        for (int i = 0; i < letterButtons.Length; i++)
        {
            if (letterButtons[i] != null)
            {
                string letter = GetLetterFromIndex(i);
                UpdateLetterButtonState(letterButtons[i], letter);
            }
        }
    }
    
    private string GetLetterFromIndex(int index)
    {
        if (index >= 0 && index < 26)
        {
            return ((char)('A' + index)).ToString();
        }
        return "A";
    }
    
    private void UpdateLetterButtonState(Button button, string letter)
    {
        if (button == null || currencyManager == null || currencyManager.allLetters == null)
            return;
        
        // Check if letter exists in dictionary
        if (!currencyManager.allLetters.ContainsKey(letter))
        {
            Debug.LogWarning($"LetterSelector: Letter '{letter}' not found in currencyManager.allLetters");
            return;
        }
        
        bool isUnlocked = currencyManager.IsLetterUnlocked(letter);
        button.interactable = isUnlocked;
        
        // Change color based on unlock status and prestige
        var colors = button.colors;
        if (isUnlocked)
        {
            // Check for prestige status
            var letterData = currencyManager.allLetters[letter];
            
            if (letterData.isGold)
            {
                // Gold color - rich gold with slight shine
                Color goldColor = new Color(1f, 0.95f, 0.4f, 1f);
                colors.normalColor = goldColor;
                colors.selectedColor = new Color(goldColor.r * 1.1f, goldColor.g * 1.1f, goldColor.b * 1.1f, 1f);
                colors.highlightedColor = new Color(goldColor.r * 1.2f, goldColor.g * 1.2f, goldColor.b * 1.2f, 1f);
            }
            else if (letterData.isSilver)
            {
                // Silver color - slightly blue-tinted silver
                Color silverColor = new Color(0.75f, 0.85f, 0.95f, 1f);
                colors.normalColor = silverColor;
                colors.selectedColor = new Color(silverColor.r * 1.1f, silverColor.g * 1.1f, silverColor.b * 1.1f, 1f);
                colors.highlightedColor = new Color(silverColor.r * 1.2f, silverColor.g * 1.2f, silverColor.b * 1.2f, 1f);
            }
            else
            {
                // Normal unlocked color
                colors.normalColor = Color.white;
                colors.selectedColor = Color.white;
            }
        }
        else
        {
            colors.normalColor = Color.gray;
            colors.selectedColor = Color.gray;
        }
        button.colors = colors;
    }
    
    // Call this method when a new letter is unlocked
    public void OnLetterUnlocked(string letter)
    {
        int index = letter[0] - 'A';
        if (index >= 0 && index < letterButtons.Length && letterButtons[index] != null)
        {
            UpdateLetterButtonState(letterButtons[index], letter);
        }
    }
} 