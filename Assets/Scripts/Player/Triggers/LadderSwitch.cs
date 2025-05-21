using amogus;
using System;
using UnityEngine;

namespace amogus
{
    public class LadderSwitch : ControllerSwitch<LadderAnimation, LadderAnimation>
    {
        public override PlayerFSM.ControllerType FromType => PlayerFSM.ControllerType.FREE_MOVE;
        public override PlayerFSM.ControllerType ToType => PlayerFSM.ControllerType.LADDER;

        public override bool enableCamera => true;
        [SerializeField]
        private LadderAnimation toLadderAnimation;
        [SerializeField]
        private LadderAnimation fromLadderAnimation;
        public override PlayerAnimation ForwardTransitionBase => toLadderAnimation;
        public override PlayerAnimation BackwardTransitionBase => fromLadderAnimation;

        [field: SerializeField] public Ladder ladder { get; private set; }
        [SerializeField] Ladder.EndType whichEnd;
        public override void TransferData(PlayerController controller)
        {
            if (controller is LadderController)
                TransferLadderData(controller as LadderController);
        }

        void TransferLadderData(LadderController controller)
        {
            controller.SetLadder(ladder);
        }

        private void OnEnable()
        {
            toLadderAnimation.LazyInitialiseFromSwitch(this, whichEnd);
            fromLadderAnimation.LazyInitialiseFromSwitch(this, whichEnd);
        }
    }
}

