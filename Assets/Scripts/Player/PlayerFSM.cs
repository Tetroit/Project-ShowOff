using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace amogus
{
    public class PlayerFSM : MonoBehaviour
    {
        [SerializeField]
        public enum ControllerType
        {
            NONE,
            FREE_MOVE,
            LADDER
        }
        public ControllerType currentControllerID;

        public ControllerType startControllerType;

        [System.Serializable]
        public struct ControllerTypeToControllerPair
        {
            public ControllerType controllerType;
            public PlayerController playerController;
        }
        [SerializeField]
        List<ControllerTypeToControllerPair> controllerList = new();
        [SerializeField]
        PlayerCamera cameraScript;
        public Dictionary<ControllerType, PlayerController> controllerDict =>
            controllerList.ToDictionary(pair => pair.controllerType, pair => pair.playerController);
        private void OnEnable()
        {
            foreach (var pair in controllerList)
            {
                if (startControllerType == pair.controllerType) pair.playerController.EnableControl();
                else pair.playerController.DisableControl();
            }
            currentControllerID = startControllerType;
        }

        public bool ValidateController(ControllerType id)
        {
            if (id == ControllerType.NONE) return true;
            if (!controllerDict.ContainsKey(id))
            {
                Debug.LogError("No controller found");
                return false;
            }
            return true;
        }
        public void SwitchController(ControllerType id, ScriptedAnimation<PlayerFSM> animation)
        {
            if (!ValidateController(id)) return;
            animation.OnEnd += () => SwitchController(id);

            if (currentControllerID != ControllerType.NONE)
            {
                Debug.Log("Entered animation");
                controllerDict[currentControllerID].DisableControl();
                DisableCamera();
                currentControllerID = ControllerType.NONE;
            }
            animation.StartAnimation(this);
        }

        public void SwitchController(ControllerType id)
        {
            Debug.Log("Changing controls");
            if (id == currentControllerID) return;
            if (!ValidateController(id)) return;

            if (currentControllerID != ControllerType.NONE)
                controllerDict[currentControllerID].DisableControl();
            else
                EnableCamera();

            if (id != ControllerType.NONE)
                controllerDict[id].EnableControl();
            currentControllerID = id;
        }

        private void OnTriggerEnter(Collider other)
        {
            ControllerSwitch sw = other.GetComponent<ControllerSwitch>();
            if (sw == null || !sw.enabled)
            {
                Debug.Log("No switch found");
                return;
            }
            if (sw.fromType != currentControllerID) return;
            Debug.Log("Transition triggered");
            if (sw.transition != null)
                SwitchController(sw.toType, sw.transition);
            else
                SwitchController(sw.toType);
        }

        public void EnableCamera()
        {
            if (cameraScript == null) return;
            cameraScript.isCinematic = false;
        }
        public void DisableCamera()
        {
            if (cameraScript == null) return;
            cameraScript.isCinematic = true;
        }
        public void ResetCamera()
        {
            if (cameraScript == null) return;
            cameraScript.ReadRotation();
        }
    }
}
