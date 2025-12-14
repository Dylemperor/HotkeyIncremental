using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject mainView;
    public GameObject letterPage;
    public GameObject numberPage;
    public GameObject automationPage;
    public GameObject titleScreen;
    public GameObject infoView;
    public GameObject settingsView;

    private void Awake()
    {
        // Ensure main view is shown by default on startup
        ShowTitleScreen();
    }

    public void ShowMainView()
    {
        mainView.SetActive(true);
        letterPage.SetActive(false);
        numberPage.SetActive(false);
        automationPage.SetActive(false);
        titleScreen.SetActive(false);
        infoView.SetActive(false);
        settingsView.SetActive(false);
    }

    public void ShowLetterPage()
    {
        mainView.SetActive(false);
        letterPage.SetActive(true);
        numberPage.SetActive(false);
        automationPage.SetActive(false);
        titleScreen.SetActive(false);
        infoView.SetActive(false);
        settingsView.SetActive(false);
    }
    
    public void ShowNumberPage()
    {
        mainView.SetActive(false);
        letterPage.SetActive(false);
        numberPage.SetActive(true);
        automationPage.SetActive(false);
        titleScreen.SetActive(false);
        infoView.SetActive(false);
        settingsView.SetActive(false);
    }
    
    public void ShowAutomationPage()
    {
        mainView.SetActive(false);
        letterPage.SetActive(false);
        numberPage.SetActive(false);
        automationPage.SetActive(true);
        titleScreen.SetActive(false);
        infoView.SetActive(false);
        settingsView.SetActive(false);
    }

    public void ShowTitleScreen()
    {
        mainView.SetActive(false);
        letterPage.SetActive(false);
        numberPage.SetActive(false);
        automationPage.SetActive(false);
        titleScreen.SetActive(true);
        infoView.SetActive(false);
        settingsView.SetActive(false);
    }
    
    public void ShowInfoView()
    {
        mainView.SetActive(false);
        letterPage.SetActive(false);
        numberPage.SetActive(false);
        automationPage.SetActive(false);
        titleScreen.SetActive(false);
        infoView.SetActive(true);
        settingsView.SetActive(false);
    }
    
    public void ShowSettingsView()
    {
        mainView.SetActive(false);
        letterPage.SetActive(false);
        numberPage.SetActive(false);
        automationPage.SetActive(false);
        titleScreen.SetActive(false);
        infoView.SetActive(false);
        settingsView.SetActive(true);
    }
}
