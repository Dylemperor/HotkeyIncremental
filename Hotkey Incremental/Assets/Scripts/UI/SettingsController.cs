using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class SettingsController : MonoBehaviour
{
    [Header("UI References")]
    public Slider updateRateSlider;
    public TMP_Text updateRateValueText;
    public TMP_Dropdown numberFormatDropdown;
    public Slider autoSaveIntervalSlider;
    public TMP_Text autoSaveIntervalValueText;
    public Button exportSaveButton;
    public Button importSaveButton;
    public Button manualSaveButton;
    public Button resetGameButton;
    public TMP_Text exportSaveText;
    public TMP_Text manualSaveStatusText;
    public TMP_InputField importSaveInput;
    public GameObject resetConfirmPanel;
    public Button resetConfirmButton;
    public Button resetCancelButton;
    
    [Header("System References")]
    public MainViewController mainViewController;
    public LetterPageController letterPageController;
    public GameController gameController;
    public WebSaveManager webSaveManager;
    public SaveManager saveManager;
    
    [Header("Settings")]
    public float minUpdateInterval = 0.01f;
    public float maxUpdateInterval = 1.0f;
    public float defaultUpdateInterval = 0.1f;
    
    public float minAutoSaveInterval = 30f;
    public float maxAutoSaveInterval = 600f; // 10 minutes
    public float defaultAutoSaveInterval = 90f;
    
    private void Start()
    {
        LoadSettings();
        SetupUI();
    }
    
    private void SetupUI()
    {
        // Update Rate Slider
        if (updateRateSlider != null)
        {
            updateRateSlider.minValue = minUpdateInterval;
            updateRateSlider.maxValue = maxUpdateInterval;
            updateRateSlider.value = PlayerPrefs.GetFloat("UpdateInterval", defaultUpdateInterval);
            updateRateSlider.onValueChanged.AddListener(OnUpdateRateChanged);
            UpdateUpdateRateDisplay();
        }
        
        // Number Format Dropdown
        if (numberFormatDropdown != null)
        {
            numberFormatDropdown.ClearOptions();
            numberFormatDropdown.AddOptions(new System.Collections.Generic.List<string> 
            { 
                "Standard (K, M, B, T)", 
                "Scientific Notation", 
                "Full Number",
                "Engineering Notation"
            });
            numberFormatDropdown.value = PlayerPrefs.GetInt("NumberFormat", 0);
            numberFormatDropdown.onValueChanged.AddListener(OnNumberFormatChanged);
        }
        
        // Auto-Save Interval Slider
        if (autoSaveIntervalSlider != null)
        {
            autoSaveIntervalSlider.minValue = minAutoSaveInterval;
            autoSaveIntervalSlider.maxValue = maxAutoSaveInterval;
            autoSaveIntervalSlider.value = PlayerPrefs.GetFloat("AutoSaveInterval", defaultAutoSaveInterval);
            autoSaveIntervalSlider.onValueChanged.AddListener(OnAutoSaveIntervalChanged);
            UpdateAutoSaveIntervalDisplay();
        }
        
        // Export Save Button
        if (exportSaveButton != null)
        {
            exportSaveButton.onClick.AddListener(ExportSaveData);
        }
        
        // Import Save Button
        if (importSaveButton != null)
        {
            importSaveButton.onClick.AddListener(ImportSaveData);
        }
        
        // Manual Save Button
        if (manualSaveButton != null)
        {
            manualSaveButton.onClick.AddListener(ManualSave);
        }
        
        // Reset Game Button
        if (resetGameButton != null)
        {
            resetGameButton.onClick.AddListener(ShowResetConfirm);
        }
        
        // Reset Confirm Buttons
        if (resetConfirmButton != null)
        {
            resetConfirmButton.onClick.AddListener(ConfirmResetGame);
        }
        
        if (resetCancelButton != null)
        {
            resetCancelButton.onClick.AddListener(CancelResetGame);
        }
        
        // Hide reset confirm panel initially
        if (resetConfirmPanel != null)
        {
            resetConfirmPanel.SetActive(false);
        }
    }
    
    private void LoadSettings()
    {
        // Settings are loaded in SetupUI
    }
    
    private void SaveSettings()
    {
        PlayerPrefs.Save();
    }
    
    // Update Rate Settings
    public void OnUpdateRateChanged(float value)
    {
        PlayerPrefs.SetFloat("UpdateInterval", value);
        PlayerPrefs.SetFloat("LetterViewUpdateInterval", value);
        PlayerPrefs.SetFloat("MainViewUpdateInterval", value);
        SaveSettings();
        
        // Apply to controllers
        if (mainViewController != null)
        {
            mainViewController.SetUpdateInterval(value);
        }
        if (letterPageController != null)
        {
            letterPageController.SetUpdateInterval(value);
        }
        
        UpdateUpdateRateDisplay();
    }
    
    private void UpdateUpdateRateDisplay()
    {
        if (updateRateValueText != null && updateRateSlider != null)
        {
            float value = updateRateSlider.value;
            float updatesPerSecond = 1f / value;
            updateRateValueText.text = $"{value:F2}s ({updatesPerSecond:F1} updates/sec)";
        }
    }
    
    // Number Format Settings
    public void OnNumberFormatChanged(int formatIndex)
    {
        PlayerPrefs.SetInt("NumberFormat", formatIndex);
        SaveSettings();
        
        // Number format is applied through NumberFormatter static class
        // You'll need to modify NumberFormatter to support different formats
        Debug.Log($"Number format changed to: {formatIndex}");
    }
    
    // Auto-Save Interval Settings
    public void OnAutoSaveIntervalChanged(float value)
    {
        PlayerPrefs.SetFloat("AutoSaveInterval", value);
        SaveSettings();
        
        // Apply to GameController
        if (gameController != null)
        {
            var field = gameController.GetType().GetField("autoSaveInterval");
            if (field != null)
            {
                field.SetValue(gameController, value);
            }
        }
        
        UpdateAutoSaveIntervalDisplay();
    }
    
    private void UpdateAutoSaveIntervalDisplay()
    {
        if (autoSaveIntervalValueText != null && autoSaveIntervalSlider != null)
        {
            float value = autoSaveIntervalSlider.value;
            if (value < 60f)
            {
                autoSaveIntervalValueText.text = $"{value:F0} seconds";
            }
            else
            {
                float minutes = value / 60f;
                autoSaveIntervalValueText.text = $"{minutes:F1} minutes";
            }
        }
    }
    
    // Export Save Data
    public void ExportSaveData()
    {
        try
        {
            string saveData = "";
            
            // Try WebSaveManager first (for web builds)
            if (webSaveManager != null)
            {
                var method = webSaveManager.GetType().GetMethod("ExportSaveData");
                if (method != null)
                {
                    saveData = (string)method.Invoke(webSaveManager, null);
                }
            }
            
            // Fallback: manually export from CurrencyManager
            if (string.IsNullOrEmpty(saveData) && gameController != null && gameController.currencyManager != null)
            {
                saveData = ExportSaveDataManually();
            }
            
            if (!string.IsNullOrEmpty(saveData))
            {
                // Copy to clipboard (web builds) or show in text field
                #if UNITY_WEBGL && !UNITY_EDITOR
                CopyToClipboard(saveData);
                if (exportSaveText != null)
                {
                    exportSaveText.text = "Save data copied to clipboard!";
                }
                #else
                if (exportSaveText != null)
                {
                    exportSaveText.text = saveData;
                }
                GUIUtility.systemCopyBuffer = saveData;
                Debug.Log("Save data copied to clipboard");
                #endif
            }
            else
            {
                if (exportSaveText != null)
                {
                    exportSaveText.text = "Failed to export save data";
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error exporting save: {e.Message}");
            if (exportSaveText != null)
            {
                exportSaveText.text = $"Error: {e.Message}";
            }
        }
    }
    
    private string ExportSaveDataManually()
    {
        if (gameController == null || gameController.currencyManager == null) return "";
        
        var saveData = new System.Collections.Generic.Dictionary<string, object>();
        foreach (var pair in gameController.currencyManager.allLetters)
        {
            var letterData = new System.Collections.Generic.Dictionary<string, object>
            {
                { "amount", pair.Value.amount },
                { "isUnlocked", pair.Value.isUnlocked },
                { "upgrades", SerializeUpgrades(pair.Value.upgrades) }
            };
            saveData[pair.Key] = letterData;
        }
        
        return JsonUtility.ToJson(saveData);
    }
    
    private string SerializeUpgrades(System.Collections.Generic.Dictionary<string, UpgradeData> upgrades)
    {
        var upgradeStrings = new System.Collections.Generic.List<string>();
        foreach (var upgrade in upgrades)
        {
            upgradeStrings.Add($"{upgrade.Key}:{upgrade.Value.level}:{upgrade.Value.effect}:{upgrade.Value.cost}");
        }
        return string.Join(";", upgradeStrings);
    }
    
    // Import Save Data
    public void ImportSaveData()
    {
        if (importSaveInput == null || string.IsNullOrEmpty(importSaveInput.text))
        {
            Debug.LogWarning("No save data to import");
            return;
        }
        
        try
        {
            string saveData = importSaveInput.text;
            
            // Try WebSaveManager first
            if (webSaveManager != null)
            {
                // We can call directly as we know the method exists now
                webSaveManager.ImportSaveData(saveData);
                Debug.Log("Save data imported via WebSaveManager");
                
                // RELOAD SCENE to ensure all managers (Currency, Number, etc.) re-initialize with the new data
                // This ensures unlocked letters/numbers are properly displayed
                UnityEngine.SceneManagement.SceneManager.LoadScene(
                    UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
                );
            }
            else
            {
                 if (exportSaveText != null) exportSaveText.text = "Error: WebSaveManager not found";
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error importing save: {e.Message}");
            if (exportSaveText != null)
            {
                exportSaveText.text = $"Import Error: {e.Message}";
            }
        }
    }
    
    // Manual Save
    public void ManualSave()
    {
        try
        {
            // Try GameController first (preferred method)
            if (gameController != null)
            {
                gameController.SaveGame();
                ShowSaveStatus("Game saved successfully!", true);
            }
            // Fallback to direct save managers
            else if (webSaveManager != null)
            {
                var method = webSaveManager.GetType().GetMethod("SaveGame");
                if (method != null)
                {
                    method.Invoke(webSaveManager, null);
                    ShowSaveStatus("Game saved successfully!", true);
                }
            }
            else if (saveManager != null)
            {
                saveManager.SaveGame();
                ShowSaveStatus("Game saved successfully!", true);
            }
            else
            {
                ShowSaveStatus("Error: No save manager found", false);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error saving game: {e.Message}");
            ShowSaveStatus($"Save Error: {e.Message}", false);
        }
    }
    
    private void ShowSaveStatus(string message, bool isSuccess)
    {
        if (manualSaveStatusText != null)
        {
            manualSaveStatusText.text = message;
            manualSaveStatusText.color = isSuccess ? Color.green : Color.red;
            
            // Clear message after 3 seconds
            CancelInvoke(nameof(ClearSaveStatus));
            Invoke(nameof(ClearSaveStatus), 3f);
        }
    }
    
    private void ClearSaveStatus()
    {
        if (manualSaveStatusText != null)
        {
            manualSaveStatusText.text = "";
        }
    }
    
    // Reset Game
    public void ShowResetConfirm()
    {
        if (resetConfirmPanel != null)
        {
            resetConfirmPanel.SetActive(true);
        }
    }
    
    public void ConfirmResetGame()
    {
        if (gameController != null)
        {
            gameController.ResetGame();
        }
        else
        {
            // Fallback reset
            if (webSaveManager != null)
            {
                var method = webSaveManager.GetType().GetMethod("ResetSave");
                if (method != null)
                {
                    method.Invoke(webSaveManager, null);
                }
            }
            else if (saveManager != null)
            {
                saveManager.ResetSave();
            }
            
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
            );
        }
    }
    
    public void CancelResetGame()
    {
        if (resetConfirmPanel != null)
        {
            resetConfirmPanel.SetActive(false);
        }
    }
    
    // Copy to clipboard for web builds
    #if UNITY_WEBGL && !UNITY_EDITOR
    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void CopyToClipboard(string text);
    #else
    private void CopyToClipboard(string text)
    {
        GUIUtility.systemCopyBuffer = text;
    }
    #endif
}

