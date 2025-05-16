using System;
using System.Collections.Generic;
using UnityEngine;

namespace amogus
{
    public class TestCutsceneTrigger : CutsceneTrigger<PlayerFSM>
    {
        [SerializeField] private TestCutsceneAnimation cutscene;
        public override ScriptedAnimation<PlayerFSM> Cutscene => cutscene;
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
            target.DisableControls();
        }
        public override void End(PlayerFSM target)
        {
            target.EnableControls();
            // Implement logic to end the animation here
        }
    }
}
