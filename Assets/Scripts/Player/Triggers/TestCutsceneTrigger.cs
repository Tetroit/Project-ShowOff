using System;
using System.Collections.Generic;
using UnityEngine;

namespace amogus
{
    public class TestCutsceneTrigger : CutsceneTrigger
    {
        [SerializeField] private TestCutsceneAnimation cutscene;
        public override PlayerAnimation Cutscene => cutscene;
        public override void TransferData(PlayerController controller)
        {
        }
    }
    [Serializable]
    public class TestCutsceneAnimation : PlayerAnimation
    {
        public Vector3 targetPos;
        Vector3 startPos;
        public override void Animate(PlayerFSM target)
        {
            target.transform.position = Vector3.Lerp(startPos, targetPos, time01);
        }
        public override void Begin(PlayerFSM target)
        {
            startPos = target.transform.position;

        }
        public override void End(PlayerFSM target)
        {
            // Implement logic to end the animation here
        }
    }
}
