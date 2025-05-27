using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Timeline;

namespace amogus
{
    public class DoorCutsceneTrigger : DoorTrigger
    {
        [SerializeField] protected TimelineAsset closingCutscene;
        [SerializeField] protected TimelineAsset openingCutscene => closingCutscene;

        [SerializeField] protected DoorCutsceneAnimation cutscene;
        [SerializeField] protected Door door;

        public override Predicate<PlayerFSM> Predicate => (PlayerFSM player) => {
            if (isLocked || cutscene.isRunningOn(door)) return false;
            return true;
        };

        PlayerAnimationBinder binder;
        bool FindBinder()
        {
            if (binder != null) return true;

            if (binder == null)
                binder = FindAnyObjectByType<PlayerAnimationBinder>();
            if (binder == null)
            {
                Debug.LogError("Target director was null", this);
                return false;
            }
            return true;
        }    
        protected override bool NullHandling(PlayerFSM target)
        {
            if (target == null)
            {
                Debug.LogError("Target object was null", this);
                return false;
            }
            if (director == null)
            {
                if (!FindBinder()) return false;
                director = binder.armDirector;
                if (director == null)
                {
                    Debug.LogError("Target director was null", this);
                    return false;
                }
            }
            if (openingCutscene == null)
            {
                if (!FindBinder()) return false;
                closingCutscene = binder.armDoorOpeningTimeline;
                if ( closingCutscene == null)
                {
                    Debug.LogError("Asset was null", this);
                    return false;
                }
            }
            return true;
        }
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

        public override void Unlock()
        {
            isLocked = false;
        }
    }
}
