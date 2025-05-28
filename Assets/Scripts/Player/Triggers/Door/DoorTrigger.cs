using UnityEngine;

namespace amogus
{
    public abstract class DoorTrigger : TimelinePlayerTrigger
    {
        [SerializeField] protected Door door;
        public string unlockCode;
        public bool isLocked = false;
        public abstract void Unlock();

    }
}
