using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class FadeIn : MonoBehaviour
{
    [SerializeField] float _fadeTime = 1f;
    Image _fadeBG;
    Tween _fadeTween;

    void OnEnable()
    {
        Debug.Log("aaa");

        if (_fadeBG == null)
            _fadeBG = GetComponent<Image>();
        
        _fadeTween?.Kill();

        // Reset alpha to 1
        Color clr = _fadeBG.color;
        clr.a = 1f;
        _fadeBG.color = clr;

        _fadeTween = _fadeBG.DOFade(0, _fadeTime);
    }
}
