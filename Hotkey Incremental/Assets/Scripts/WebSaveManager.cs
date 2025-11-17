using UnityEngine;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;

public class WebSaveManager : MonoBehaviour
{
    public CurrencyManager currencyManager;

    private const string SaveKeyPrefix = "HotkeyIncremental_Letter_";
    private const string SaveVersionKey = "HotkeyIncremental_SaveVersion";
    private const int CurrentSaveVersion = 1;

    // JavaScript interop for web builds
    #if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void SaveToLocalStorage(string key, string value);

    [DllImport("__Internal")]
    private static extern IntPtr LoadFromLocalStorage(string key);

    [DllImport("__Internal")]
    private static extern bool HasKeyInLocalStorage(string key);

    [DllImport("__Internal")]
    private static extern void DeleteFromLocalStorage(string key);

    [DllImport("__Internal")]
    private static extern void ClearLocalStorage();
    #else
    // Fallback for editor/standalone builds
    private static void SaveToLocalStorage(string key, string value)
    {
        PlayerPrefs.SetString(key, value);
        PlayerPrefs.Save();
    }

    private static string LoadFromLocalStorage(string key)
    {
        return PlayerPrefs.GetString(key, "");
    }

    private static bool HasKeyInLocalStorage(string key)
    {
        return PlayerPrefs.HasKey(key);
    }

    private static void DeleteFromLocalStorage(string key)
    {
        PlayerPrefs.DeleteKey(key);
    }

    private static void ClearLocalStorage()
    {
        PlayerPrefs.DeleteAll();
    }
    #endif

    private void Start()
    {
        // Load game on start
        LoadGame();
    }

    public void SaveGame()
    {
        if (currencyManager == null)
        {
            Debug.LogError("WebSaveManager: CurrencyManager is null, cannot save!");
            return;
        }

        try
        {
            // Save version
            SaveToLocalStorage(SaveVersionKey, CurrentSaveVersion.ToString());

            // Save all letter data
            foreach (var pair in currencyManager.allLetters)
            {
                string key = SaveKeyPrefix + pair.Key;
                string upgradeData = SerializeUpgrades(pair.Value.upgrades);
                string saveData = $"{pair.Value.amount}|{(pair.Value.isUnlocked ? 1 : 0)}|{upgradeData}";
                SaveToLocalStorage(key, saveData);
            }

            Debug.Log("Game Saved successfully to localStorage");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error saving game: {e.Message}");
        }
    }

    public void LoadGame()
    {
        if (currencyManager == null)
        {
            Debug.LogError("WebSaveManager: CurrencyManager is null, cannot load!");
            return;
        }

        try
        {
            // Check if save exists
            bool hasSave = false;
            foreach (var pair in currencyManager.allLetters)
            {
                string key = SaveKeyPrefix + pair.Key;
                if (HasKeyInLocalStorage(key))
                {
                    hasSave = true;
                    break;
                }
            }

            if (!hasSave)
            {
                Debug.Log("No save data found, starting fresh game");
                return;
            }

            // Load all letter data
            foreach (var pair in currencyManager.allLetters)
            {
                string key = SaveKeyPrefix + pair.Key;
                if (HasKeyInLocalStorage(key))
                {
                    #if UNITY_WEBGL && !UNITY_EDITOR
                    IntPtr ptr = LoadFromLocalStorage(key);
                    if (ptr != IntPtr.Zero)
                    {
                        string saveData = Marshal.PtrToStringUTF8(ptr);
                        Marshal.FreeHGlobal(ptr);
                    #else
                    string saveData = LoadFromLocalStorage(key);
                    if (!string.IsNullOrEmpty(saveData))
                    {
                    #endif
                        string[] parts = saveData.Split('|');
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
            }

            Debug.Log("Game Loaded successfully from localStorage");
        }
        catch (Exception e)
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
        try
        {
            // Delete all save keys
            foreach (var pair in currencyManager.allLetters)
            {
                string key = SaveKeyPrefix + pair.Key;
                DeleteFromLocalStorage(key);
            }
            DeleteFromLocalStorage(SaveVersionKey);
            Debug.Log("Save reset - localStorage cleared");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error resetting save: {e.Message}");
        }
    }

    // Export save data as JSON string (for backup/transfer)
    public string ExportSaveData()
    {
        if (currencyManager == null) return "";

        var saveData = new Dictionary<string, object>();
        foreach (var pair in currencyManager.allLetters)
        {
            var letterData = new Dictionary<string, object>
            {
                { "amount", pair.Value.amount },
                { "isUnlocked", pair.Value.isUnlocked },
                { "upgrades", SerializeUpgrades(pair.Value.upgrades) }
            };
            saveData[pair.Key] = letterData;
        }

        return JsonUtility.ToJson(saveData);
    }

    // Import save data from JSON string
    public void ImportSaveData(string jsonData)
    {
        // Implementation for importing save data
        // This can be used for save file transfer between devices
    }
}

