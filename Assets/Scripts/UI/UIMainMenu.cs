using UnityEngine;
using UnityEngine.SceneManagement;

public class UIMainMenu : MonoBehaviour
{
    // Pages
    [SerializeField] GameObject _mMainPage;
    [SerializeField] GameObject _mSettingsPage;
    [SerializeField] GameObject _mCreditsPage;

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
                Application.OpenURL("https://www.linkedin.com/in/stefan-carpeliuc-b2880427b/");
                break;
            case "Connor":
                Application.OpenURL("https://www.linkedin.com/in/connor-smith-59222a253/");
                break;
            case "Lluis":
                Application.OpenURL("https://www.linkedin.com/in/lluis-alguersuari-4831562b9/");
                break;
            case "Rianne":
                Application.OpenURL("https://www.linkedin.com/in/rianne-jongerius-0410411ab/");
                break;
            case "Kyra":
                Application.OpenURL("https://www.linkedin.com/in/kyra-zendman-2467bb223/");
                break;
            case "Jade":
                Application.OpenURL("https://www.linkedin.com/in/jade-izmirlioglu/");
                break;
        }
    }
    #endregion
}
