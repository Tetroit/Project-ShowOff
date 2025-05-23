using UnityEngine;
using System.Collections;

public class ButtonsHUD : MonoBehaviour
{
    // Canvases
    [SerializeField] GameObject _menuCanvas;
    [SerializeField] GameObject _gameCanvas;

    // Pages
    [SerializeField] GameObject _unpausedView;
    [SerializeField] GameObject _pausedView;
    [SerializeField] GameObject _pMainPage;
    [SerializeField] GameObject _pControlsPage;
    [SerializeField] GameObject _pAudioPage;


    public void QuitMenu()
    { // Return to menu stuff here
        _menuCanvas.SetActive(true);
        _pausedView.SetActive(false);
        _gameCanvas.SetActive(false);
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
        _pausedView.SetActive(false);
        _unpausedView.SetActive(true);
    }
    public void PauseGame()
    {
        _pausedView.SetActive(true);
        _unpausedView.SetActive(false);
    }

}
