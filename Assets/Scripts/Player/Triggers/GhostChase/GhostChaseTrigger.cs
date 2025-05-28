using System;
using UnityEngine;


namespace amogus
{
    public class GhostChaseTrigger : DoorTrigger
    {
        [SerializeField] GhostChaseAnimation cutscene;
        [SerializeField] DoorCutsceneTrigger endDoor;
        public bool CanUnlcok;

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

            if (cutscene.animation == 1)
            {
                endDoor.OnAnimationEnd.AddListener(StopGhostFirstTime);
                
            }

            if (cutscene.animation != 1 && CanUnlcok) isLocked = false; 
        }

        public void StopGhostFirstTime()
        {
            cutscene.miller.StopFollowing();
            cutscene.miller.gameObject.SetActive(false);
            endDoor.OnAnimationEnd.RemoveListener(StopGhostFirstTime);

            Debug.Log("Stopping ghost");
        }
    }
}
