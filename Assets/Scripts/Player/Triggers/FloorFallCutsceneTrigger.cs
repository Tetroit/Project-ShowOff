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
        [SerializeField] float _repositionTime = .5f;
        PlayableDirector director;

        Vector3 endPos;
        Quaternion endRot;
        Vector3 startPos;
        Quaternion startRot;

        bool targetAligned = false;
        public override void Animate(PlayerFSM target)
        {
            if (time < _repositionTime)
            {
                target.transform.SetPositionAndRotation
                (
                    Vector3.Lerp(startPos, endPos, time), 
                    Quaternion.Slerp(startRot, endRot, time)
                );
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

            // Save current position
            target.transform.GetPositionAndRotation
            (
                out Vector3 originalPos,
                out Quaternion originalRot
            );
            totalDuration = (float)director.duration + _repositionTime;
            // Sample the Timeline at the beginning
            director.time = 0;
            director.Evaluate();

            // Get the animated starting pose
            startPos = originalPos;
            startRot = originalRot;

            endPos = target.transform.position;
            endRot = target.transform.rotation;
            // Restore original transform to avoid snapping early
            target.transform.SetPositionAndRotation
            (
                originalPos,
                originalRot
            );
            target.DisableControls();
        }
        public override void End(PlayerFSM target)
        {
            director.Stop();
            target.EnableControls();
        }

        public void DrawGizmos()
        {
        }
    }
}
