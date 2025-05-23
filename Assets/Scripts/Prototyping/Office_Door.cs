using UnityEngine;

namespace amogus
{
    public class Office_Door : MonoBehaviour
    {
        [SerializeField] private DoorCutsceneTrigger doorToUnlock;

        private bool unlocked = false;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.O))
            {
                Unlock();
            }
        }
        public void Unlock()
        {
            if (unlocked || doorToUnlock == null)
                return;

            doorToUnlock.isLocked = false;
            unlocked = true;

            Debug.Log($"Door '{doorToUnlock.name}' unlocked via DoorUnlockHandle.");
        }
    }
}