using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Unity.VisualScripting;
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
        public bool inAnimation = false;
        public bool isMoving
        {
            get
            {
                if (currentControllerID == ControllerType.NONE) return false;
                if (!ValidateController(currentControllerID)) return false;
                return controllerDict[currentControllerID].isMoving;
            }
        }
        public Dictionary<ControllerType, PlayerController> controllerDict =>
            controllerList.ToDictionary(pair => pair.controllerType, pair => pair.playerController);

        void Awake()
        {
            PlayerInputHandler.Create();
        }
        private void OnEnable()
        {
            foreach (var pair in controllerList)
            {
                if (startControllerType == pair.controllerType) pair.playerController.EnableControl();
                else pair.playerController.DisableControl();
            }
            currentControllerID = startControllerType;
            inAnimation = false;
        }

        public bool ValidateController(ControllerType id)
        {
            if (id == ControllerType.NONE) return true;
            if (!controllerDict.ContainsKey(id))
            {
                Debug.LogError($"No controller found of {id} type found");
                return false;
            }
            return true;
        }

        public PlayerController GetController(ControllerType id)
        {
            if (!ValidateController(id) || id == ControllerType.NONE) return null;
            return controllerDict[id];
        }
        public void DisableControls()
        {
            EnterAnimation();
            controllerDict[currentControllerID].DisableControl();

        }
        public void EnableControls()
        {
            controllerDict[currentControllerID].EnableControl();
            ExitAnimation();
        }
        public void SwitchController(ControllerType id, ScriptedAnimation<PlayerFSM> animation)
        {
            if (!ValidateController(id)) return;
            animation.OnEnd += () => { 
                SwitchController(id); 
                ExitAnimation(); 
            };

            if (currentControllerID != ControllerType.NONE)
            {
                Debug.Log("Entered animation");
                controllerDict[currentControllerID].DisableControl();
                DisableCamera();
            }
            EnterAnimation();
            animation.StartAnimation(this);
            currentControllerID = ControllerType.NONE;
        }

        public void SwitchController(ControllerType id)
        {
            Debug.Log("Changing controls");
            if (id == currentControllerID) return;
            if (!ValidateController(id)) return;

            if (currentControllerID != ControllerType.NONE)
                controllerDict[currentControllerID].DisableControl();

            if (id != ControllerType.NONE)
            {
                controllerDict[id].EnableControl();
            }
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
            if (inAnimation) return;
            ActivateSwitch(sw);
        }


        public void ActivateSwitch(ControllerSwitch sw)
        {
            if (sw.FromType == currentControllerID)
            {
                Debug.Log("Forward transition triggered");
                if (sw.useForwardAnimation)
                    SwitchController(sw.ToType, sw.ForwardTransitionBase);
                else
                    SwitchController(sw.ToType);
                sw.TransferData(controllerDict[sw.ToType]);
            }
            else if (sw.ToType == currentControllerID)
            {

                Debug.Log("Backward transition triggered");
                if (sw.useBackwardAnimation)
                    SwitchController(sw.FromType, sw.BackwardTransitionBase);
                else
                    SwitchController(sw.ToType);
                sw.TransferData(controllerDict[sw.ToType]);
            }
        }
        public void EnterAnimation()
        {
            inAnimation = true;
            DisableCamera();
        }
        public void ExitAnimation()
        {
            inAnimation = false;
            EnableCamera();
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
        public void ReadCamera()
        {
            if (cameraScript == null) return;
            cameraScript.ReadRotation();
        }
    }
}
