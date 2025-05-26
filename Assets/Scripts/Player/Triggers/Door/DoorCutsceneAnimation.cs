using System;
using UnityEngine;

namespace amogus
{
    [Serializable]
    public class DoorCutsceneAnimation : ScriptedAnimation<Door>
    {
        Quaternion startRotation;
        Quaternion targetRotation;
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
