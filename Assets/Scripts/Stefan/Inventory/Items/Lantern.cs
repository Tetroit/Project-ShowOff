using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

public class Lantern : InventoryItemView
{
    [SerializeField] GameObject[] UI_Icons;
    [Header("Light related")]
    [SerializeField] bool _enabled;
    [SerializeField] Light _light;
    [SerializeField] float _yLevel;
    [SerializeField] float _blendDistance = .5f;
    [SerializeField] float _underGroundLumen = 1;
    [SerializeField] float _surfaceLumen = 1;

    [SerializeField] float _flickeringScale;
    [SerializeField] float _flickeringSpeed;

    [Header("For debug")]
    [SerializeField] float t = 1;
    
    public override void Select()
    {
        base.Select();
        foreach (var icon in UI_Icons) icon.SetActive(true);
    }

    public override void Deselect()
    {
        base.Deselect();
        foreach (var icon in UI_Icons) icon.SetActive(false);

    }

    void Update()
    {
        if (!_enabled) return;
        float currentY = transform.position.y;

        currentY = math.clamp(currentY, _yLevel - _blendDistance, _yLevel + _blendDistance);

        currentY -= _yLevel; 
        
        t = currentY / _blendDistance;
        _light.intensity = math.lerp(_underGroundLumen, _surfaceLumen, t);


        float height = _flickeringScale * Mathf.PerlinNoise(Time.time * _flickeringSpeed, 0.0f);
        _light.intensity += height;
    }
}
