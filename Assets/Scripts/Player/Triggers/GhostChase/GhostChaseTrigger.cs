using System;
using UnityEngine;


namespace amogus
{
    public class GhostChaseTrigger : DoorTrigger
    {
        [SerializeField] GhostChaseAnimation cutscene;
        public override Predicate<PlayerFSM> Predicate => (PlayerFSM player) =>
        {
            return (cutscene.animation == 1 || !isLocked) && !cutscene.isRunningOn(door);
        };

        public override void Trigger()
        {
            base.Trigger();
            cutscene.totalDuration = (float)asset.duration;
            cutscene.StartAnimation(door);
        }
        public override void Unlock()
        {
            cutscene.animation++;

            if (cutscene.animation != 1) isLocked = false; 
        }
    }
}
