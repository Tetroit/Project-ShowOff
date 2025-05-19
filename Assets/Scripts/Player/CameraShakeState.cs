using UnityEngine;

[CreateAssetMenu(fileName = "CameraShakeState", menuName = "Player/CameraShakeState")]
public class CameraShakeState : ScriptableObject
{
    public bool resetOnDoingNothing;
    public AnimationCurve XShake;
    public AnimationCurve YShake;
    public float rotationIntensity;
    public float frequency;
}
