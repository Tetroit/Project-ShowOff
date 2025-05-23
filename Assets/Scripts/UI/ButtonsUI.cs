using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonsUI : MonoBehaviour
{
    // Pages
    [SerializeField] private GameObject _mMainPage;
    [SerializeField] private GameObject _mSettingsPage;
    [SerializeField] private GameObject _mCreditsPage;

    public void QuitGame()
    {
        Application.Quit();
    }
    public void ClickPlay()
    {   
        // Do start of game stuff here
        SceneManager.LoadScene(1);
    }
    public void ClickSettings()
    {
        _mMainPage.SetActive(false);
        _mSettingsPage.SetActive(true);
    }
    public void ClickCredits()
    {
        _mMainPage.SetActive(false);
        _mCreditsPage.SetActive(true);
    }
    public void ClickMainMenu()
    {
        _mCreditsPage.SetActive(false);
        _mSettingsPage.SetActive(false);
        _mMainPage.SetActive(true);
    }



    #region Portfolio links
    public void OpenPortfolio(string pName)
    {
        Debug.Log("opening link for: " + pName);
        switch (name)
        {
            case "Ivan":
                Application.OpenURL("https://www.linkedin.com/in/ivan-shychynov-b6a9482ba/");
                break;
            case "Stefan":
                Application.OpenURL("");
                break;
            case "Connor":
                Application.OpenURL("");
                break;
            case "Lluis":
                Application.OpenURL("");
                break;
            case "Rianne":
                Application.OpenURL("https://www.linkedin.com/in/rianne-jongerius-0410411ab/");
                break;
            case "Kyra":
                Application.OpenURL("");
                break;
            case "Jade":
                Application.OpenURL("");
                break;
        }
    }
    #endregion
}
