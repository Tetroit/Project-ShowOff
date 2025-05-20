using System;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.InputSystem;

namespace amogus
{
    public class DoorCutsceneTrigger : CutsceneTrigger<PlayerFSM, Door>
    {
        [SerializeField] private DoorCutsceneAnimation cutscene;

        public override ScriptedAnimation<Door> Cutscene => cutscene;
        public override Predicate<PlayerFSM> Predicate => (PlayerFSM player) => {
            if (target.isLocked) return false;
            return true;
        };
    }
    [Serializable]
    public class DoorCutsceneAnimation : ScriptedAnimation<Door>
    {
        Vector3 startPos;

        [SerializeField] Quaternion startRotation;
        [SerializeField] Quaternion targetRotation;
        public override void Animate(Door target)
        {
            target.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, time01);
        }
        public override void Begin(Door target)
        {
            startRotation = target.GetStartRotation();
            targetRotation = target.GetTargetRotation();

        }
        public override void End(Door target)
        {
            if (target.isOpen)
            {
                target.Close();
            }
            else
            {
                target.Open();
            }
        }
    }
}
