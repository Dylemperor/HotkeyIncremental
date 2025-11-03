using UnityEngine;
using TMPro;
using System.Collections;

public class LetterPageController : MonoBehaviour
{
    public TMP_Text letterAmountText;
    public TMP_Text letterProductionText;
    public CurrencyManager currencyManager;
    public ProductionManager productionManager;
    public string currentLetter = "A";
    
    private void Start()
    {
        StartCoroutine(UpdateLetterDisplay());
    }
    
    IEnumerator UpdateLetterDisplay()
    {
        while (true)
        {
            UpdateLetterInfo();
            yield return new WaitForSeconds(0.5f);
        }
    }
    
    public void LoadLetter(string letter)
    {
        currentLetter = letter;
        UpdateLetterInfo();
    }
    
    private void UpdateLetterInfo()
    {
        if (currencyManager.allLetters.ContainsKey(currentLetter))
        {
            var data = currencyManager.allLetters[currentLetter];
            
            // Update amount text
            if (letterAmountText != null)
            {
                letterAmountText.text = $"{currentLetter}: {NumberFormatter.Format(data.amount)}";
            }
            
            // Update production text
            if (letterProductionText != null)
            {
                double productionRate = productionManager.GetProductionRate(currentLetter);
                letterProductionText.text = $"Production: {NumberFormatter.Format(productionRate)}/s";
            }
        }
    }
}
