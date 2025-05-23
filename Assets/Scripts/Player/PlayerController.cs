using System;
using UnityEngine;
namespace amogus
{
    public abstract class PlayerController : MonoBehaviour
    {
        public abstract void EnableControl();
        public abstract void DisableControl();
        public abstract bool isMoving { get; }
        public Action<CameraWalkingShake.State> OnCameraShakeChange;
        public Action<float> OnFOVChange;
    }
}