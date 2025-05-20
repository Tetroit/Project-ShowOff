using amogus;
using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

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
    }
    [Serializable]
    public class FloorFallCutsceneAnimation : ScriptedAnimation<PlayerFSM>
    {
        [SerializeField] PlayableDirector director;
        public override void Animate(PlayerFSM target)
        {
            director = target.GetComponent<PlayableDirector>();
            director.Play();
        }
        public override void Begin(PlayerFSM target)
        {

        }
        public override void End(PlayerFSM target)
        {
            director.Stop();
        }
    }
}
