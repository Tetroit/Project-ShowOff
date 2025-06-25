using DG.Tweening;
using FMODUnity;
using UnityEngine;
using UnityEngine.InputSystem;

public class VoiceMailController : MonoBehaviour
{
    [SerializeField] InputActionReference _playVoiceMail;
    [SerializeField] TextRunner _voiceMailRunner;
    [SerializeField] EventReference _ringtone;
    [SerializeField] CanvasGroup _ui;
    [SerializeField] float _fadeSpeed = .8f;
    [SerializeField] float _hangUpTimeAfter = 5f;

    void Start()
    {
        if (AudioManager.instance != null && !_ringtone.IsNull)
        {
            AudioManager.instance.PlayOneShot(_ringtone, Vector3.zero);
        }

        _playVoiceMail.action.started += AnswerPhone;
        _playVoiceMail.action.Enable();
        
        Invoke(nameof(CancelPhone), _hangUpTimeAfter);
    }

    void CancelPhone()
    {
        _playVoiceMail.action.started -= AnswerPhone;
        //_playVoiceMail.action.Disable();
        _ui.DOFade(0, _fadeSpeed);
    }

    void AnswerPhone(InputAction.CallbackContext context)
    {
        _voiceMailRunner.DisplayText();

        _ui.DOFade(0,_fadeSpeed);
        _playVoiceMail.action.started -= AnswerPhone;
        //_playVoiceMail.action.Disable();

    }
}
