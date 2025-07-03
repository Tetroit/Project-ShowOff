using DG.Tweening;
using FMOD.Studio;
using FMODUnity;
using Mono.Cecil;
using UnityEngine;
using UnityEngine.InputSystem;

public class VoiceMailController : MonoBehaviour
{
    [SerializeField] InputActionReference _playVoiceMail;
    [SerializeField] TextRunner _voiceMailRunner;
    [SerializeField] FMODUnity.EventReference _ringtone;
    private EventInstance _eventInstance;
    [SerializeField] CanvasGroup _ui;
    [SerializeField] float _fadeSpeed = .8f;
    [SerializeField] float _hangUpTimeAfter = 5f;

    void Start()
    {
        if (AudioManager.instance != null && !_ringtone.IsNull)
        {
            _eventInstance = RuntimeManager.CreateInstance(_ringtone);
            _eventInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
            _eventInstance.start();
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
        _eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        _eventInstance.release();
        _voiceMailRunner.DisplayText();
        _eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        _ui.DOFade(0,_fadeSpeed);
        _playVoiceMail.action.started -= AnswerPhone;
        //_playVoiceMail.action.Disable();

    }
}
