using Unity.Mathematics;
using UnityEngine;

public class LightChange : MonoBehaviour
{
    [Header("Light related")]
    [SerializeField] Light _light;
    [SerializeField] float _yLevel;
    [SerializeField] float _blendDistance = .5f;
    [SerializeField] float _underGroundLumen = 1;
    [SerializeField] float _surfaceLumen = 1;
    [SerializeField] bool _enableFlickering;
    [SerializeField] float _flickeringScaleUnder = 300;
    [SerializeField] float _flickeringScaleOutside = 0.3f;
    [SerializeField] float _flickeringSpeed;

    [Header("For debug")]
    [SerializeField] float t = 1;

    void Update()
    {
        float currentY = transform.position.y;

        currentY = math.clamp(currentY, _yLevel - _blendDistance, _yLevel + _blendDistance);

        currentY -= _yLevel;

        t = currentY / _blendDistance;
        _light.intensity = math.lerp(_underGroundLumen, _surfaceLumen, t);

        if (!_enableFlickering) return;

        var currentFlicer = math.lerp(_flickeringScaleUnder, _flickeringScaleOutside, t);

        float height = currentFlicer * -Mathf.PerlinNoise(Time.time * _flickeringSpeed, 0.0f);
        _light.intensity += height;
    }


}
