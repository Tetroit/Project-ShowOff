using System;
using UnityEngine;

namespace amogus
{
    public class ClimbCutsceneTrigger : CutsceneTrigger<PlayerFSM>
    {
        [field: SerializeField] public Vector3 targetPosLocal { get; private set; }
        [SerializeField] private ClimbCutsceneAnimation cutscene;
        public override ScriptedAnimation<PlayerFSM> Cutscene => cutscene;
        public override Predicate<PlayerFSM> Predicate => (PlayerFSM player) => {
            return true;
        };

        [ExecuteInEditMode]
        private void OnDrawGizmos()
        {
            Gizmos.matrix = Matrix4x4.Translate(transform.position);
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(targetPosLocal, 0.1f);
        }
    }
    [Serializable]
    public class ClimbCutsceneAnimation : ScriptedAnimation<PlayerFSM>
    {
        Vector3 startPos;
        [SerializeField] ClimbCutsceneTrigger trigger;
        Vector3 targetPos => trigger.targetPosLocal + trigger.transform.position;
        public override void Animate(PlayerFSM target)
        {
            target.transform.position = Vector3.Lerp(startPos, targetPos, time01);
        }
        public override void Begin(PlayerFSM target)
        {
            startPos = target.transform.position;
            //target.DisableControls();
        }
        public override void End(PlayerFSM target)
        {
            //target.EnableControls();
        }

    }
}
