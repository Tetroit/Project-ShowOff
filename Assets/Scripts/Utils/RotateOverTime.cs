using UnityEngine;

public class RotateOverTime : MonoBehaviour
{
    public float Speed = 1;
    public Vector3 Rotation;
    public RotationData RotationData;

    bool _wasSet;

    void Awake()
    {
        Rotation.Normalize();
    }

    //using fixed update so it responds to scaled time
    void FixedUpdate()
    {
        if( RotationData != null && !_wasSet )
        {
            Rotation = RotationData.Euler;
            Rotation.Normalize();
            _wasSet = true;
        }
        else if( RotationData == null )
        {
            _wasSet = false;
        }
        float speedMult = RotationData == null ? 1 : RotationData.Speed;
        transform.Rotate(Rotation, Speed * speedMult * Time.fixedDeltaTime);
    }
}
