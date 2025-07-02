using amogus;
using UnityEngine;
using UnityEngine.Playables;

namespace amogus
{
    public class LadderController : PlayerController
    {
        [SerializeField] PlayerFSM playerFSM;
        [SerializeField] Transform target;
        [SerializeField] Transform cameraTransform;
        [SerializeField] float speed;
        [SerializeField] PlayableDirector _director;
        [SerializeField] InventoryView _inventory;
        [SerializeField] InventoryController _inventoryController;
        [SerializeField] float _animSpeed = 1;
        [SerializeField] Vector3 _armOffset;

        string _previousItemName;

        Ladder ladderContext;
        Vector3 facing => cameraTransform.rotation * Vector3.forward;
        public override void DisableControl()
        {
            enabled = false;
            Debug.Log("ladder controls disabled");
            if(_inventoryController != null) 
                _inventoryController.enabled = true;
            if(_director != null)
                _director.gameObject.SetActive(false);

            if (_previousItemName != null)
                _inventory.SelectItem(_previousItemName);
            _previousItemName = null;
        }

        public override void EnableControl()
        {
            OnCameraShakeChange?.Invoke(CameraWalkingShake.State.LADDER);
            enabled = true;
            Debug.Log("ladder controls enabled");

            //play ladder animation
            _director.gameObject.SetActive(true);
            _previousItemName = _inventory.GetCurrentItem().name;
            _inventory.SelectItem("NoArm");
            _inventoryController.enabled = false;
        }

        public override bool isMoving
        {
            get
            {
                return (playerAction > 0.1 || playerAction < -0.1);
            }
        }
        float playerAction;
        public void Update()
        {
            bool flipCamera = Vector3.Dot(facing, ladderContext.facing * Vector3.forward) < 0;
            
            playerAction = PlayerInputHandler.Instance.Move.y;
            target.transform.position += Time.deltaTime * speed * playerAction * (flipCamera? -1 : 1) * ladderContext.GetDir();
            //target.transform.position += Time.deltaTime * speed * playerAction * Vector3.up;

            float currentHeight = ladderContext.GetHeight(target.position);

            if (playerAction * (flipCamera ? -1 : 1) < 0 && currentHeight <= 0)
                playerFSM.ActivateSwitch(ladderContext.startLadderSwitch);

            if (playerAction * (flipCamera ? -1 : 1) > 0 && currentHeight >= 1)
                playerFSM.ActivateSwitch(ladderContext.endLadderSwitch);

            _director.transform.localPosition = target.localPosition + _armOffset;
            _director.transform.forward = -ladderContext.transform.right;
            if(playerAction != 0)
            {
                double timeCopy = _director.time;
                timeCopy += Time.deltaTime * _animSpeed * playerAction;
                if(timeCopy <= 0 )
                {
                    timeCopy += 1.5;    
                }
                _director.time = timeCopy;
                _director.Evaluate();
            }
        }

        public void SetLadder(Ladder ladder)
        {
            ladderContext = ladder;
        }
    }
}

