using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject mainView;
    public GameObject letterPage;
    public GameObject numberPage;
    public GameObject automationPage;
    public GameObject infoView;
    public GameObject settingsView;

    private void Awake()
    {
        // Ensure main view is shown by default on startup
        ShowMainView();
    }

    public void ShowMainView()
    {
        if (mainView != null) mainView.SetActive(true);
        if (letterPage != null) letterPage.SetActive(false);
        if (numberPage != null) numberPage.SetActive(false);
        if (automationPage != null) automationPage.SetActive(false);
        if (infoView != null) infoView.SetActive(false);
        if (settingsView != null) settingsView.SetActive(false);
    }

    public void ShowLetterPage()
    {
        if (mainView != null) mainView.SetActive(false);
        if (letterPage != null) letterPage.SetActive(true);
        if (numberPage != null) numberPage.SetActive(false);
        if (automationPage != null) automationPage.SetActive(false);
        if (infoView != null) infoView.SetActive(false);
        if (settingsView != null) settingsView.SetActive(false);
    }
    
    public void ShowNumberPage()
    {
        if (mainView != null) mainView.SetActive(false);
        if (letterPage != null) letterPage.SetActive(false);
        if (numberPage != null) numberPage.SetActive(true);
        if (automationPage != null) automationPage.SetActive(false);
        if (infoView != null) infoView.SetActive(false);
        if (settingsView != null) settingsView.SetActive(false);
    }
    
    public void ShowAutomationPage()
    {
        if (mainView != null) mainView.SetActive(false);
        if (letterPage != null) letterPage.SetActive(false);
        if (numberPage != null) numberPage.SetActive(false);
        if (automationPage != null) automationPage.SetActive(true);
        if (infoView != null) infoView.SetActive(false);
        if (settingsView != null) settingsView.SetActive(false);
    }
    
    public void ShowInfoView()
    {
        if (mainView != null) mainView.SetActive(false);
        if (letterPage != null) letterPage.SetActive(false);
        if (numberPage != null) numberPage.SetActive(false);
        if (automationPage != null) automationPage.SetActive(false);
        if (infoView != null) infoView.SetActive(true);
        if (settingsView != null) settingsView.SetActive(false);
    }
    
    public void ShowSettingsView()
    {
        if (mainView != null) mainView.SetActive(false);
        if (letterPage != null) letterPage.SetActive(false);
        if (numberPage != null) numberPage.SetActive(false);
        if (automationPage != null) automationPage.SetActive(false);
        if (infoView != null) infoView.SetActive(false);
        if (settingsView != null) settingsView.SetActive(true);
    }
}
