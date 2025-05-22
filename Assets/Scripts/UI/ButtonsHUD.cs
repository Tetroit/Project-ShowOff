using UnityEngine;
using System.Collections;

public class ButtonsHUD : MonoBehaviour
{
    // Canvases
    [SerializeField] private GameObject _menuCanvas;
    [SerializeField] private GameObject _gameCanvas;

    // Pages
    [SerializeField] private GameObject _unpausedView;
    [SerializeField] private GameObject _pausedView;
    [SerializeField] private GameObject _pMainPage;
    [SerializeField] private GameObject _pControlsPage;
    [SerializeField] private GameObject _pAudioPage;

    // Variables
    private bool _isPaused = false;

    public void QuitMenu()
    { // Return to menu stuff here
        _isPaused = false;
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
        _isPaused = false;
        _pControlsPage.SetActive(false);
        _pAudioPage.SetActive(false);
        _pMainPage.SetActive(true);
        _pausedView.SetActive(false);
        _unpausedView.SetActive(true);
    }
    public void PauseGame()
    {
        _isPaused = true;
        _pausedView.SetActive(true);
        _unpausedView.SetActive(false);
    }

}
