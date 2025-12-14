using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class AutomationView : MonoBehaviour
{
    public AutomationManager automationManager;
    public NumberManager numberManager;
    
    [Header("First Half Automation")]
    public Toggle firstHalfToggle;
    public TMP_Text firstHalfStatusText;
    
    [Header("Second Half Automation")]
    public Toggle secondHalfToggle;
    public TMP_Text secondHalfStatusText;
    
    [Header("Speed Upgrade")]
    public Button speedUpgradeButton;
    public TMP_Text speedUpgradeButtonText;
    public TMP_Text speedDisplayText;
    
    [Header("Update Settings")]
    public float updateInterval = 0.1f;
    
    private void Start()
    {
        // Setup toggles
        if (firstHalfToggle != null)
        {
            firstHalfToggle.onValueChanged.AddListener(OnFirstHalfToggled);
            firstHalfToggle.isOn = automationManager != null && automationManager.IsFirstHalfEnabled();
        }
        
        if (secondHalfToggle != null)
        {
            secondHalfToggle.onValueChanged.AddListener(OnSecondHalfToggled);
            secondHalfToggle.isOn = automationManager != null && automationManager.IsSecondHalfEnabled();
        }
        
        // Setup speed upgrade button
        if (speedUpgradeButton != null)
        {
            speedUpgradeButton.onClick.AddListener(OnSpeedUpgradeClicked);
        }
        
        StartCoroutine(UpdateAutomationView());
    }
    
    private IEnumerator UpdateAutomationView()
    {
        while (true)
        {
            RefreshAutomationUI();
            yield return new WaitForSeconds(updateInterval);
        }
    }
    
    private void RefreshAutomationUI()
    {
        if (automationManager == null || numberManager == null)
            return;
        
        // Update first half status
        if (firstHalfStatusText != null)
        {
            bool hasUpgrade = numberManager.HasAutomationUpgrade(2);
            bool isEnabled = automationManager.IsFirstHalfEnabled();
            
            if (!hasUpgrade)
            {
                firstHalfStatusText.text = "Purchase First Half Automation upgrade in Number 2";
            }
            else if (isEnabled)
            {
                firstHalfStatusText.text = "Enabled - Auto-buying upgrades for A-M";
            }
            else
            {
                firstHalfStatusText.text = "Disabled";
            }
            
            if (firstHalfToggle != null)
            {
                firstHalfToggle.interactable = hasUpgrade;
            }
        }
        
        // Update second half status
        if (secondHalfStatusText != null)
        {
            bool hasUpgrade = numberManager.HasAutomationUpgrade(3);
            bool isEnabled = automationManager.IsSecondHalfEnabled();
            
            if (!hasUpgrade)
            {
                secondHalfStatusText.text = "Purchase Second Half Automation upgrade in Number 3";
            }
            else if (isEnabled)
            {
                secondHalfStatusText.text = "Enabled - Auto-buying upgrades for N-Z";
            }
            else
            {
                secondHalfStatusText.text = "Disabled";
            }
            
            if (secondHalfToggle != null)
            {
                secondHalfToggle.interactable = hasUpgrade;
            }
        }
        
        // Update speed display
        if (speedDisplayText != null)
        {
            double speed = automationManager.GetAutomationSpeed();
            speedDisplayText.text = $"Automation Speed: Every {speed:F1} seconds";
        }
        
        // Update speed upgrade button
        if (speedUpgradeButton != null && speedUpgradeButtonText != null)
        {
            var (number, amount) = numberManager.GetAutomationSpeedCost();
            bool canAfford = numberManager.GetNumberAmount(number) >= amount;
            
            speedUpgradeButton.interactable = canAfford;
            
            double currentSpeed = automationManager.GetAutomationSpeed();
            double newSpeed = Mathf.Max(0.5f, (float)(currentSpeed / 2.0));
            if (newSpeed < 1.0f && newSpeed > 0.5f)
            {
                newSpeed = 0.5f;
            }
            
            speedUpgradeButtonText.text = $"Upgrade Speed\nCost: {amount} {number}s\n({currentSpeed:F1}s → {newSpeed:F1}s)";
        }
    }
    
    private void OnFirstHalfToggled(bool value)
    {
        if (automationManager != null)
        {
            automationManager.SetFirstHalfEnabled(value);
        }
    }
    
    private void OnSecondHalfToggled(bool value)
    {
        if (automationManager != null)
        {
            automationManager.SetSecondHalfEnabled(value);
        }
    }
    
    public void OnSpeedUpgradeClicked()
    {
        if (numberManager != null)
        {
            bool success = numberManager.PurchaseAutomationSpeedUpgrade();
            if (success)
            {
                RefreshAutomationUI();
            }
        }
    }
}

