using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

namespace amogus
{
    public abstract class DoorTrigger : TimelinePlayerTrigger
    {
        public string unlockCode;
        public bool isLocked = false;
        [SerializeField] protected Door door;
        [Header("Events")]
        public UnityEvent<bool> OnTryUnlock;

        protected override void TryTrigger(PlayerFSM other)
        {
            if (!NullHandling(other)) return;

            if (isLocked)
                TryUnlocking(other);
            else
                Open(other);
        }
        protected virtual void TryUnlocking(PlayerFSM other)
        {
            var item = FindAnyObjectByType<InventoryController>()?.GetCurrentInventory()?.GetCurrentItem();
            if (item == null || item is not Key)
            {
                Debug.Log("No item to unlock with", this);
                OnTryUnlock?.Invoke(false);
                return;
            }
            if (item is Key)
            {
                var key = item as Key;
                if (key.doorCode == unlockCode && enabled)
                {
                    Debug.Log("Unlocked", this);
                    isLocked = false;
                    OnTryUnlock?.Invoke(true);
                    Unlock();
                }
                else
                {
                    Debug.Log("Failed to unlock", this);
                    OnTryUnlock?.Invoke(false);
                }
            }
        }
        protected virtual void Open(PlayerFSM other)
        {
            if (Predicate(other) && enabled && !isLocked)
            {
                Debug.Log("Triggered", this);
                triggerObject = other;
                Trigger();
                OnTrigger?.Invoke();

            }
        }
        public abstract void Unlock();

    }
}
