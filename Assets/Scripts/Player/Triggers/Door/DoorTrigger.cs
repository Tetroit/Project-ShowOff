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
        public UnityEvent OnFailUnlock;
        public UnityEvent OnUnlock;

        protected override void TryTrigger(PlayerFSM other)
        {
            if (!NullHandling(other)) return;

            if (!(useRaycast && !RaycastCheck(other.camera.transform)))
            {
                if (isLocked)
                    TryUnlocking(other);
                else
                    Open(other);
            }
        }
        protected virtual void TryUnlocking(PlayerFSM other)
        {
            var controller = FindAnyObjectByType<InventoryController>(FindObjectsInactive.Include);
            var keyInventoryTr = controller.transform.FindDeepChild("KeyInventory");
            var inventory = keyInventoryTr.gameObject.GetComponent<InventoryView>();

            Key correctKey = inventory.First(k => (k as Key)?.doorCode == unlockCode) as Key;
            if (correctKey == null)
            {
                Debug.Log("No item to unlock with", this);
                OnFailUnlock?.Invoke();
                return;
            }
            if (correctKey)
            {
                if (enabled)
                {
                    Debug.Log("Unlocked", this);
                    isLocked = false;
                    Unlock();
                    OnUnlock?.Invoke();

                    Open(other);
                }
                else
                {
                    Debug.Log("Failed to unlock", this);
                    OnFailUnlock?.Invoke();
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
