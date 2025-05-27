using System;
using UnityEngine;

namespace amogus
{
    public class TimelinePlayerTrigger : TimelineTrigger<PlayerFSM>
    {
        public override Predicate<PlayerFSM> Predicate => (PlayerFSM player) => {
            return true;
        };

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
