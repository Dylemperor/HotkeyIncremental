using UnityEngine;
using System.Collections.Generic;

public class UpgradeManager : MonoBehaviour
{
    public CurrencyManager currencyManager;

    [System.Serializable]
    public class Upgrade
    {
        public string letter;
        public string name;
        public double cost;
        public float multiplier;
        public bool isPurchased;
    }

    public List<Upgrade> allUpgrades = new List<Upgrade>();

    // Update UpgradeManager to work with CurrencyData upgrades
    public void PurchaseUpgrade(string letter, string upgradeName)
    {
        if (currencyManager.allLetters.ContainsKey(letter))
        {
            var currencyData = currencyManager.allLetters[letter];
            var upgrade = currencyData.upgrades[upgradeName];
            
            if (currencyData.amount >= upgrade.cost)
            {
                currencyData.amount -= upgrade.cost;
                upgrade.Upgrade(); // This calls the Upgrade() method in UpgradeData
                Debug.Log($"Purchased {upgradeName} for {letter}");
            }
        }
    }

    public List<Upgrade> GetAvailableUpgrades(string letter)
    {
        return allUpgrades.FindAll(u => u.letter == letter && !u.isPurchased);
    }
}
