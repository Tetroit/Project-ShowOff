using System;
using UnityEngine;

namespace amogus
{
    public class TimelinePlayerTrigger : TimelineTrigger<PlayerFSM>
    {
        public override Predicate<PlayerFSM> Predicate => (PlayerFSM player) => {
            return true;
        };

        [SerializeField] protected PlayerAnimationBinder binder;
        [SerializeField] string playerDirectorName = "Director";
        [SerializeField] string playerTimelineName = "Timeline";
        public string PlayerDirectorName
        {
            get => playerDirectorName;
            set => playerDirectorName = value;
        }
        public string PlayerTimelineName
        {
            get => playerTimelineName;
            set => playerTimelineName = value;
        }
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
                director = binder.GetDirector(PlayerDirectorName);
                if (director == null)
                {
                    Debug.LogError("Target director was null", this);
                    return false;
                }
            }
            if (asset == null)
            {
                if (!FindBinder()) return false;
                asset = binder.GetTimeline(PlayerTimelineName);
                if (asset == null)
                {
                    Debug.LogError("Asset was null", this);
                    return false;
                }
            }
            return true;
        }
        public override void Trigger()
        {
            base.Trigger();
            triggerObject.DisableControls();
        }
        [ExecuteAlways]
        private void OnDrawGizmos()
        {
        }
        protected override void AnimationEnd()
        {
            triggerObject.EnableControls();
        }
    }
}
