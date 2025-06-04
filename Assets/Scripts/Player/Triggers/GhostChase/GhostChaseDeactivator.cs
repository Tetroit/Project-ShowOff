using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using UnityEngine.ProBuilder.Shapes;


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
            var door = GetComponent<DoorCutsceneTrigger>();
            if (door == null)
            {
                Debug.LogError("DoorCutsceneTrigger component is missing on the GameObject.");
                return;
            }
            door.OnTrigger.AddListener(Trigger);
        }
        public void Unhook()
        {
            var door = GetComponent<DoorCutsceneTrigger>();
            if (door == null)
            {
                Debug.LogError("DoorCutsceneTrigger component is missing on the GameObject.");
                return;
            }
            door.OnTrigger.RemoveListener(Trigger);
        }
        public void OnDisable()
        {
            Unhook();
        }
    }
}
