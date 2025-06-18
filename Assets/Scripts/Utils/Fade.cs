using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Fade : MonoBehaviour
{
    public float Time;
    CanvasGroup _image;

    void OnEnable()
    {
        _image = GetComponent<CanvasGroup>();
        _image.alpha = 0;
        _image.DOFade(1, Time).SetUpdate(true);    
    }

}
