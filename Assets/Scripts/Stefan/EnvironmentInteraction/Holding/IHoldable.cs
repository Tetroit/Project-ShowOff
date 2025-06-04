using UnityEngine;

public interface IHoldable : IInteractable
{
    Transform Self { get; }
    Vector3 GetInitialPosition();
    Quaternion GetInitialRotation();
}
