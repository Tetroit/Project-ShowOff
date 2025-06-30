using DG.Tweening;
using UnityEngine;

public class Fade : MonoBehaviour
{
    public float Time;
    public FadeMode Mode = FadeMode.In;
    CanvasGroup _image;
    Tween _fadeTween;

    public enum FadeMode
    {
        In,
        Out
    }

    void OnEnable()
    {
        if(_image == null)
            _image = GetComponent<CanvasGroup>();
        _fadeTween?.Kill();

        switch (Mode)
        {
            case FadeMode.In:
                _image.alpha = 1;
                _fadeTween = _image.DOFade(0, Time).SetUpdate(true);
                break;
            case FadeMode.Out:
                _image.alpha = 0;
                _fadeTween = _image.DOFade(1, Time).SetUpdate(true);
                break;
        }
        
    }

}
