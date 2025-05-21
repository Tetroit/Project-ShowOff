using amogus;
using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;

namespace amogus
{
    public class FloorFallCutsceneTrigger : CutsceneTrigger<PlayerFSM>
    {
        [SerializeField] private FloorFallCutsceneAnimation cutscene;

        public Matrix4x4 offset => Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
        public Vector3 targetPos;
        public Quaternion targetFacing;
        public Vector3 targetPosGS => transform.TransformPoint(targetPos);
        public Quaternion targetFacingGS => targetFacing;
        public override ScriptedAnimation<PlayerFSM> Cutscene => cutscene;
        public override Predicate<PlayerFSM> Predicate => (PlayerFSM player) => {
            return true;
        };

        public override void Trigger()
        {
            base.Trigger();
        }
        [ExecuteAlways]
        private void OnDrawGizmos()
        {
            cutscene.DrawGizmos();
        }
    }
    [Serializable]
    public class FloorFallCutsceneAnimation : ScriptedAnimation<PlayerFSM>
    {
        [SerializeField] FloorFallCutsceneTrigger trigger;
        [SerializeField] PlayableDirector director;

        Vector3 startPos;
        Quaternion startRot;

        bool targetAligned = false;
        public override void Animate(PlayerFSM target)
        {
            if (time < 1)
            {
                target.transform.position = Vector3.Lerp(startPos, trigger.targetPosGS, time);
                target.transform.rotation = Quaternion.Slerp(startRot, trigger.targetFacingGS, time);
            }
            else
            {
                if (!targetAligned)
                {
                    targetAligned = true;
                    director.Play();
                }
            }
        }
        public override void Begin(PlayerFSM target)
        {
            director = target.GetComponent<PlayableDirector>();
            startPos = target.transform.position;
            startRot = target.transform.rotation;
            target.DisableControls();
        }
        public override void End(PlayerFSM target)
        {
            director.Stop();
            target.EnableControls();
        }

        public void DrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(trigger.targetPosGS, 0.2f);
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(trigger.targetPosGS, trigger.targetPosGS + trigger.targetFacingGS * Vector3.forward);
        }
    }
}
