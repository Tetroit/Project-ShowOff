using UnityEngine;

[CreateAssetMenu(fileName = "RotationData", menuName = "Stefan/RotationData")]
public class RotationData : ScriptableObject
{
    public Vector3 Euler;
    public float Speed = 1;

    public void ChangeSpeed(float speed)
    {
        Speed = speed;
    }
}
