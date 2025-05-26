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
}
