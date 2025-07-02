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

        Ladder ladderContext;
        Vector3 facing => cameraTransform.rotation * Vector3.forward;

        [SerializeField] float xRange;
        [SerializeField] float yMin;
        [SerializeField] float yMax;
        public override void DisableControl()
        {
            enabled = false;
            Debug.Log("ladder controls disabled");
            playerFSM.DisableCameraXConstraint();

            playerFSM.SetYCamConstraint(-70, 70);
        }

        public override void EnableControl()
        {
            OnCameraShakeChange?.Invoke(CameraWalkingShake.State.LADDER);
            enabled = true;
            Debug.Log("ladder controls enabled");
            playerFSM.EnableCameraXConstraint(ladderContext.facing.eulerAngles.y);

            playerFSM.SetYCamConstraint(yMin, yMax);
            playerFSM.SetXCamConstraint(xRange);
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
            target.transform.position += Time.deltaTime * speed * playerAction * ladderContext.GetDir();
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

