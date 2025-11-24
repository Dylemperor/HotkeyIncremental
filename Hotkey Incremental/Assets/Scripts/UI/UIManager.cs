using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject mainView;
    public GameObject letterPage;
    public GameObject titleScreen;
    public GameObject infoView;
    public GameObject settingsView;

    public void ShowMainView()
    {
        mainView.SetActive(true);
        letterPage.SetActive(false);
        titleScreen.SetActive(false);
        infoView.SetActive(false);
        settingsView.SetActive(false);
    }

    public void ShowLetterPage()
    {
        mainView.SetActive(false);
        letterPage.SetActive(true);
        titleScreen.SetActive(false);
        infoView.SetActive(false);
        settingsView.SetActive(false);
    }

    public void ShowTitleScreen()
    {
        mainView.SetActive(false);
        letterPage.SetActive(false);
        titleScreen.SetActive(true);
        infoView.SetActive(false);
        settingsView.SetActive(false);
    }
    
    public void ShowInfoView()
    {
        mainView.SetActive(false);
        letterPage.SetActive(false);
        titleScreen.SetActive(false);
        infoView.SetActive(true);
        settingsView.SetActive(false);
    }
    
    public void ShowSettingsView()
    {
        mainView.SetActive(false);
        letterPage.SetActive(false);
        titleScreen.SetActive(false);
        infoView.SetActive(false);
        settingsView.SetActive(true);
    }
}
