using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndScroll : MonoBehaviour
{
    [SerializeField] List<ScrollRect> _scrolls;
    [SerializeField] float _pause;
    [SerializeField] float _endPause;
    [SerializeField] float _scrollTimeMultimplier;
    [SerializeField] AnimationCurve _scrollCurve;
    [SerializeField] UnityEvent BeforePause;
    void OnEnable()
    {
        StartCoroutine(Scroll(_pause, _endPause, _scrollTimeMultimplier));    
    }

    IEnumerator Scroll(float startPause, float endPause,float time)
    {
        yield return new WaitForSeconds(startPause);
        float startTime = 0;

        while(startTime < _scrollTimeMultimplier)
        {
            yield return null;

            float pos01 = startTime / _scrollTimeMultimplier;
            pos01 = 1 - pos01;
            foreach (var scroll in _scrolls)
            {
                scroll.verticalNormalizedPosition = _scrollCurve.Evaluate(pos01);
            }
            startTime += Time.deltaTime;
        }
        BeforePause?.Invoke();
        yield return new WaitForSeconds(endPause);
        SceneManager.LoadScene(0);
    }
}
