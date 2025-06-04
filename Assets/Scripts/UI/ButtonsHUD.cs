using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonsHUD : MonoBehaviour
{

    // Pages
    [SerializeField] GameObject _pMainPage;
    [SerializeField] GameObject _pControlsPage;
    [SerializeField] GameObject _pAudioPage;


    public void QuitMenu()
    { // Return to menu stuff here
        SceneManager.LoadScene(0);
    }

    public void ClickControls()
    {
        _pMainPage.SetActive(false);
        _pControlsPage.SetActive(true);
    }
    public void ClickAudio()
    {
        _pMainPage.SetActive(false);
        _pAudioPage.SetActive(true);
    }
    public void ClickMain()
    {
        _pControlsPage.SetActive(false);
        _pAudioPage.SetActive(false);
        _pMainPage.SetActive(true);
    }
    public void UnpauseGame()
    {
        _pControlsPage.SetActive(false);
        _pAudioPage.SetActive(false);
        _pMainPage.SetActive(true);
    }

    void OnDisable()
    {
        UnpauseGame();
    }
}
