using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace amogus
{
    public interface IAnimationRoot
    {
        public Transform animationRoot { get; }
    }
    public class PlayerFSM : MonoBehaviour, IAnimationRoot
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
        CameraWalkingShake shake;

        public Camera Camera => cameraScript != null ? cameraScript.cam : null;

        [field: SerializeField] public Transform animationRoot { get; private set; }
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
            shake = GetComponentInChildren<CameraWalkingShake>();
            foreach (var pair in controllerList)
            {
                pair.playerController.OnCameraShakeChange += SetShake;
                pair.playerController.OnFOVChange += cameraScript.ChangeFOV;
                if (startControllerType == pair.controllerType) pair.playerController.EnableControl();
                else pair.playerController.DisableControl();
            }
            currentControllerID = startControllerType;
            inAnimation = false;
        }

        private void OnDisable()
        {
            foreach (var pair in controllerList)
            {
                pair.playerController.DisableControl();
                pair.playerController.OnCameraShakeChange -= SetShake;
                pair.playerController.OnFOVChange -= cameraScript.ChangeFOV;
            }
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
            if (currentControllerID == ControllerType.NONE) return;
            EnterAnimation();
            controllerDict[currentControllerID].DisableControl();

        }
        public void EnableControls()
        {
            if ( currentControllerID == ControllerType.NONE) return;

            controllerDict[currentControllerID].EnableControl();
            ExitAnimation();
        }

        public bool IsWalkingControler() => currentControllerID == ControllerType.FREE_MOVE;


        public bool IsCurrentEnabled()
        {
            if (currentControllerID == ControllerType.NONE) return false;

            return controllerDict[currentControllerID].isEnabled;
        }

        public void SwitchController(ControllerType id, ScriptedAnimation<PlayerFSM> animation, bool enableCamera = false)
        {
            if (!ValidateController(id)) return;
            animation.OnEnd.AddListener( () => { 
                SwitchController(id); 
                ExitAnimation(enableCamera);
            });

            if (currentControllerID != ControllerType.NONE)
            {
                Debug.Log("Entered animation");
                controllerDict[currentControllerID].DisableControl();
            }
            EnterAnimation(!enableCamera);
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
            if (inAnimation) return;
            ControllerSwitch sw = other.GetComponent<ControllerSwitch>();
            if (sw != null && sw.enabled)
            {
                ActivateSwitch(sw);
            }
        }


        public void ActivateSwitch(ControllerSwitch sw)
        {
            if (sw.FromType == currentControllerID)
            {
                Debug.Log("Forward transition triggered");
                if (sw.useForwardAnimation)
                    SwitchController(sw.ToType, sw.ForwardTransitionBase, sw.enableCamera);
                else
                    SwitchController(sw.ToType);
                sw.TransferData(controllerDict[sw.ToType]);
            }
            else if (sw.ToType == currentControllerID)
            {

                Debug.Log("Backward transition triggered");
                if (sw.useBackwardAnimation)
                    SwitchController(sw.FromType, sw.BackwardTransitionBase, sw.enableCamera);
                else
                    SwitchController(sw.ToType);
                sw.TransferData(controllerDict[sw.ToType]);
            }
        }
        public void EnterAnimation(bool disableCamera = true)
        {
            inAnimation = true;
            if (disableCamera)
                DisableCamera();
        }
        public void ExitAnimation(bool enableCamera = true)
        {
            inAnimation = false;
            if (enableCamera)
            {
                EnableCamera();
                ReadCamera();
            }
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

        public void SetShake(CameraWalkingShake.State shakeID)
        {
            if (shake != null)
                shake.ChangeState(shakeID);
        }
        public void EnableCameraXConstraint(float facing)
        {
            if (cameraScript == null) return;
            cameraScript.constraintX = true;
            cameraScript.constraintMiddle = facing;

        }
        public void DisableCameraXConstraint()
        {
            if (cameraScript == null) return;
            cameraScript.constraintX = false;

        }

        public void SetYCamConstraint(float bottom, float top)
        {
            cameraScript.minAngle = bottom;
            cameraScript.maxAngle = top;
        }

        internal void SetXCamConstraint(float angle)
        {
            cameraScript.xAngleRange = angle;
        }
    }
}
