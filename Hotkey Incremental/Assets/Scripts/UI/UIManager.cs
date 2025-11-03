using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject mainView;
    public GameObject letterPage;
    public GameObject titleScreen;

    public void ShowMainView()
    {
        mainView.SetActive(true);
        letterPage.SetActive(false);
        titleScreen.SetActive(false);
    }

    public void ShowLetterPage()
    {
        mainView.SetActive(false);
        letterPage.SetActive(true);
        titleScreen.SetActive(false);
    }

    public void ShowTitleScreen()
    {
        mainView.SetActive(false);
        letterPage.SetActive(false);
        titleScreen.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
