using amogus;
using UnityEngine;

namespace amogus
{
    public class LadderController : PlayerController
    {
        [SerializeField] PlayerFSM playerFSM;
        [SerializeField] Transform target; 
        [SerializeField] float speed;

        [SerializeField] Ladder ladderContext;
        public override void DisableControl()
        {
            enabled = false;
            Debug.Log("ladder controls disabled");
        }

        public override void EnableControl()
        {
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

