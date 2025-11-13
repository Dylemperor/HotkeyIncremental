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
        if (button != null)
        {
            bool isUnlocked = currencyManager.IsLetterUnlocked(letter);
            button.interactable = isUnlocked;
            
            // Change color based on unlock status
            var colors = button.colors;
            if (isUnlocked)
            {
                colors.normalColor = Color.white;
                colors.selectedColor = Color.white;
            }
            else
            {
                colors.normalColor = Color.gray;
                colors.selectedColor = Color.gray;
            }
            button.colors = colors;
        }
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