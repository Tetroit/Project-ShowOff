using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class QuickTimeEventIndicator : MonoBehaviour
{
    string actionName = "Interact";

    float _fac = 0;
    float _pressFac = 0;
    public float pulseStrength = 0.5f;
    public float coolSpeed = 1f;
    public int pressCount = 1;
    [SerializeField] UnityEngine.UI.Image image;

    Coroutine coroutine;

    private void OnEnable()
    {
        InputSystem.actions.FindActionMap("Player").FindAction(actionName).started += OnPress;
        coroutine = StartCoroutine(Pulse());
        _fac = 0;
        _pressFac = 0;
    }
    public void OnDisable()
    {
        InputSystem.actions.FindActionMap("Player").FindAction(actionName).started -= OnPress;
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }
        _fac = 0;
    }
    private void Update()
    {
        transform.localScale = Vector3.one * (1 + _fac);
        image.fillAmount = _pressFac;
    }
    void OnPress(InputAction.CallbackContext context)
    {
        _fac += pulseStrength;
        if (_fac > 1)
            _fac = 1;
        _pressFac += 1 / (float)pressCount;
        if (_pressFac > 1)
            _pressFac = 1;
    }
    IEnumerator Pulse()
    {
        _fac = 0;
        while (true)
        {
            _fac -= Time.deltaTime * coolSpeed;
            if (_fac < 0)
                _fac = 0;
            yield return null;
        }
    }

    public void SmoothDisable()
    {
        var group = GetComponentInChildren<CanvasGroup>();
        var tween = DOTween.To(() => group.alpha, x => group.alpha = x, 0, 1);
        tween.onComplete += OnDisableAnimEnd;

        void OnDisableAnimEnd()
        {
            tween.onComplete -= OnDisableAnimEnd;
            gameObject.SetActive(false);
        }
    }
}
