using amogus;
using UnityEngine;

namespace amogus
{
    public class LadderController : PlayerController
    {
        [SerializeField] PlayerFSM playerFSM;
        [SerializeField] Transform target;
        [SerializeField] Transform cameraTransform;
        [SerializeField] float speed;

        [SerializeField] Ladder ladderContext;
        Vector3 facing => cameraTransform.rotation * Vector3.forward;
        public override void DisableControl()
        {
            enabled = false;
            Debug.Log("ladder controls disabled");
        }

        public override void EnableControl()
        {
            OnCameraShakeChange?.Invoke(CameraWalkingShake.State.LADDER);
            enabled = true;
            Debug.Log("ladder controls enabled");
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

            if (playerAction < 0 && ladderContext.GetHeight(target.position) <= 0)
                playerFSM.ActivateSwitch(ladderContext.startLadderSwitch);

            if (playerAction > 0 && ladderContext.GetHeight(target.position) >= 1)
                playerFSM.ActivateSwitch(ladderContext.endLadderSwitch);
        }

        public void SetLadder(Ladder ladder)
        {
            ladderContext = ladder;
        }
    }
}

