using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using FMODUnity;

public class ConstantBlinker : MonoBehaviour
{
    [SerializeField, Range(0, 1f)] float _minWeight;
    [SerializeField, Range(0, 1f)] float _maxWeight;

    [SerializeField] Vector2 _minStateDuration = new(5f, 10f);
    [SerializeField] Vector2 _maxStateDuration = new(0.1f, .4f);

    [SerializeField] EventReference Heartbeat;
    [SerializeField] EventReference Inhale;
    [SerializeField] EventReference Exhale;

    Volume _volume;
    Coroutine _coroutine;

    void OnEnable()
    {
        if (_volume == null) _volume = GetComponentInChildren<Volume>(true);
        if (_volume == null) return;

        _coroutine = StartCoroutine(SetMin(Random.Range(_minStateDuration.x, _minStateDuration.y)));
    }

    void OnDisable()
    {
        if (_coroutine != null)
            StopCoroutine(_coroutine);
    }

    IEnumerator SetMin(float time)
    {
        AudioManager.instance.PlayOneShot(Heartbeat, transform.position);
        AudioManager.instance.PlayOneShot(Inhale, transform.position);
        DOTween.To(() => _volume.weight, (f) => _volume.weight = f, _minWeight, time);
        yield return new WaitForSeconds(time);
        _coroutine = StartCoroutine(SetMax(Random.Range(_maxStateDuration.x, _maxStateDuration.y)));
    }

    IEnumerator SetMax(float time)
    {
        AudioManager.instance.PlayOneShot(Heartbeat, transform.position);
        AudioManager.instance.PlayOneShot(Exhale, transform.position);
        DOTween.To(() => _volume.weight, (f) => _volume.weight = f, _maxWeight, time);

        yield return new WaitForSeconds(time);
        _coroutine = StartCoroutine(SetMin(Random.Range(_minStateDuration.x, _minStateDuration.y)));

    }
}
