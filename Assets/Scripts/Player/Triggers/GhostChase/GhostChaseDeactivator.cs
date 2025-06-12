using System;
using UnityEngine;
using UnityEngine.Events;

namespace amogus
{
    [Serializable]
    public class GhostChaseDeactivator : MonoBehaviour
    {
        public MillerController miller;

        public UnityEvent OnTrigger;
        public void Trigger()
        {
            miller.gameObject.SetActive(false);
            OnTrigger?.Invoke();
        }
        public void Hook()
        {
            //if (!TryGetComponent<DoorCutsceneTrigger>(out var door))
            //{
            //    Debug.LogError("DoorCutsceneTrigger component is missing on the GameObject.");
            //    return;
            //}
            //door.OnTrigger.AddListener(Trigger);
        }
        public void Unhook()
        {
            //if (!TryGetComponent<DoorCutsceneTrigger>(out var door))
            //{
            //    Debug.LogError("DoorCutsceneTrigger component is missing on the GameObject.");
            //    return;
            //}
            //door.OnTrigger.RemoveListener(Trigger);
        }
        public void OnDisable()
        {
            Unhook();
        }
    }
}
