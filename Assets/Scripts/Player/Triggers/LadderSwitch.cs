using amogus;
using UnityEngine;

namespace amogus
{
    public class LadderSwitch : ControllerSwitch<LadderAnimation, LadderAnimation>
    {
        public override PlayerFSM.ControllerType FromType => PlayerFSM.ControllerType.FREE_MOVE;
        public override PlayerFSM.ControllerType ToType => PlayerFSM.ControllerType.LADDER;

        [SerializeField]
        private LadderAnimation toLadderAnimation;
        [SerializeField]
        private LadderAnimation fromLadderAnimation;
        public override PlayerAnimation ForwardTransitionBase => toLadderAnimation;
        public override PlayerAnimation BackwardTransitionBase => fromLadderAnimation;
    }
}

