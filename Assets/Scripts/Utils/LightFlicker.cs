using System.Collections;
using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    [SerializeField] Vector2 _ligtOnDurationMinMax = new(5f,10f);
    [SerializeField] Vector2 _lightOutDurationMinMax = new(0.1f,.4f);
    [SerializeField] Light _light;
    private void Start()
    {
        StartCoroutine(LightOn(Random.Range(_ligtOnDurationMinMax.x, _ligtOnDurationMinMax.y)));   
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
