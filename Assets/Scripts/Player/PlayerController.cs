using UnityEngine;
namespace amogus
{
    public abstract class PlayerController : MonoBehaviour
    {
        public abstract void EnableControl();
        public abstract void DisableControl();

        public abstract bool isMoving { get; }
}
}