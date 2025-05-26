using System;
using UnityEngine;
using UnityEngine.Timeline;

namespace amogus
{
    public class DoorCutsceneTrigger : TimelinePlayerTrigger
    {
        [SerializeField] protected TimelineAsset closingCutscene;
        [SerializeField] protected TimelineAsset openingCutscene => closingCutscene;

        [SerializeField] protected DoorCutsceneAnimation cutscene;
        [SerializeField] protected Door door;
        public string unlockCode;
        public bool isLocked = false;

        public override Predicate<PlayerFSM> Predicate => (PlayerFSM player) => {
            if (isLocked || cutscene.isRunningOn(door)) return false;
            return true;
        };

        public override void Trigger()
        {
            if (!door.isOpen)
                asset = openingCutscene;
            else
                asset = closingCutscene;

            base.Trigger();

            cutscene.totalDuration = (float)asset.duration;
            cutscene.StartAnimation(door);
        }
        protected override void AnimationEnd()
        {
            base.AnimationEnd();
        }
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
