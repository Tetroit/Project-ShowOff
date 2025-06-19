using DG.Tweening;
using TMPro;
using UnityEngine;

public class TextFade : MonoBehaviour
{
    [SerializeField] float _time;
    [SerializeField] Ease _ease;
    TextMeshPro _text;
    private void Awake()
    {
        _text = GetComponent<TextMeshPro>();
    }
    private void OnEnable()
    {
        var startColor = _text.color;
        startColor.a = 0f;
        _text.color = startColor;

        var endColor = startColor;
        endColor.a = 1f;
        _text.DOColor(endColor, _time).SetEase(_ease);
    }
}
