using DG.Tweening;
using TMPro;
using UnityEngine;

public class TextFade : MonoBehaviour
{
    [SerializeField] float _time;
    TextMeshPro _text;
    private void Awake()
    {
        _text = GetComponent<TextMeshPro>();
    }
    private void OnEnable()
    {
        _text.alpha = 0.0f;
        _text.DOFade(1, _time);
    }
}
