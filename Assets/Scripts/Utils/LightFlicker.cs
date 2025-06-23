using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    [SerializeField] Vector2 _ligtOnDurationMinMax = new(5f,10f);
    [SerializeField] Vector2 _lightOutDurationMinMax = new(0.1f,.4f);
    [SerializeField] Light _light;
    Coroutine _coroutine;
    void OnEnable()
    {
        if(_light ==  null) _light = GetComponentInChildren<Light>(true);
        if (_light == null) return;

        _coroutine =StartCoroutine(LightOn(Random.Range(_ligtOnDurationMinMax.x, _ligtOnDurationMinMax.y)));   
    }

    void OnDisable()
    {
        if(_coroutine != null)
        StopCoroutine(_coroutine);
    }

    IEnumerator LightOn(float time)
    {
        _light.enabled = true;
        yield return new WaitForSeconds(time);
        StartCoroutine(LightOff(Random.Range(_lightOutDurationMinMax.x, _lightOutDurationMinMax.y)));
    }

    IEnumerator LightOff(float time)
    {
        _light.enabled = false;
        yield return new WaitForSeconds(time);
        StartCoroutine(LightOn(Random.Range(_ligtOnDurationMinMax.x, _ligtOnDurationMinMax.y)));

    }
}
