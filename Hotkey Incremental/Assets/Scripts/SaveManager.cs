using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class SaveManager : MonoBehaviour
{
    public CurrencyManager currencyManager;

    private const string SaveKeyPrefix = "Letter_";
    private const string UpgradeKeyPrefix = "Upgrade_";

    public void SaveGame()
    {
        if (currencyManager == null)
        {
            Debug.LogError("SaveManager: CurrencyManager is null, cannot save!");
            return;
        }
        
        try
        {
            foreach (var pair in currencyManager.allLetters)
            {
                string key = SaveKeyPrefix + pair.Key;
                string upgradeData = SerializeUpgrades(pair.Value.upgrades);
                PlayerPrefs.SetString(key, $"{pair.Value.amount}|{(pair.Value.isUnlocked ? 1 : 0)}|{upgradeData}");
            }

            PlayerPrefs.Save();
            Debug.Log("Game Saved successfully");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error saving game: {e.Message}");
        }
    }

    public void LoadGame()
    {
        if (currencyManager == null)
        {
            Debug.LogError("SaveManager: CurrencyManager is null, cannot load!");
            return;
        }
        
        try
        {
            foreach (var pair in currencyManager.allLetters)
            {
                string key = SaveKeyPrefix + pair.Key;
                if (PlayerPrefs.HasKey(key))
                {
                    string[] parts = PlayerPrefs.GetString(key).Split('|');
                    if (parts.Length >= 2)
                    {
                        if (double.TryParse(parts[0], out double amount))
                            pair.Value.amount = amount;

                        pair.Value.isUnlocked = parts[1] == "1";
                        
                        // Load upgrade data if available
                        if (parts.Length >= 3)
                        {
                            DeserializeUpgrades(pair.Value.upgrades, parts[2]);
                        }
                    }
                }
            }

            Debug.Log("Game Loaded successfully");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error loading game: {e.Message}");
        }
    }

    private string SerializeUpgrades(Dictionary<string, UpgradeData> upgrades)
    {
        var upgradeStrings = new List<string>();
        foreach (var upgrade in upgrades)
        {
            upgradeStrings.Add($"{upgrade.Key}:{upgrade.Value.level}:{upgrade.Value.effect}:{upgrade.Value.cost}");
        }
        return string.Join(";", upgradeStrings);
    }

    private void DeserializeUpgrades(Dictionary<string, UpgradeData> upgrades, string upgradeData)
    {
        if (string.IsNullOrEmpty(upgradeData)) return;
        
        string[] upgradeStrings = upgradeData.Split(';');
        foreach (string upgradeString in upgradeStrings)
        {
            string[] parts = upgradeString.Split(':');
            if (parts.Length >= 4 && upgrades.ContainsKey(parts[0]))
            {
                var upgrade = upgrades[parts[0]];
                if (int.TryParse(parts[1], out int level))
                    upgrade.level = level;
                if (double.TryParse(parts[2], out double effect))
                    upgrade.effect = effect;
                if (double.TryParse(parts[3], out double cost))
                    upgrade.cost = cost;
            }
        }
    }

    public void ResetSave()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("Save reset");
    }
}
