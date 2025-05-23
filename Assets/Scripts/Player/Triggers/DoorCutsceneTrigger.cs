﻿using System;
using UnityEngine;

namespace amogus
{
    public class DoorCutsceneTrigger : CutsceneTrigger<PlayerFSM, Door>
    {
        [SerializeField] private DoorCutsceneAnimation cutscene;
        public string unlockCode;
        public bool isLocked = false;

        public override ScriptedAnimation<Door> Cutscene => cutscene;
        public override Predicate<PlayerFSM> Predicate => (PlayerFSM player) => {
            if (isLocked) return false;
            return true;
        };
    }
    [Serializable]
    public class DoorCutsceneAnimation : ScriptedAnimation<Door>
    {
        [SerializeField] Quaternion startRotation;
        [SerializeField] Quaternion targetRotation;
        public override void Animate(Door target)
        {
            target.transform.localRotation = Quaternion.Slerp(startRotation, targetRotation, time01);
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
