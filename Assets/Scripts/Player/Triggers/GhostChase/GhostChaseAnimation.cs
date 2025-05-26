using System;
using UnityEngine;


namespace amogus
{
    [Serializable]
    public class GhostChaseAnimation : ScriptedAnimation<Door>
    {
        [SerializeField] MillerController miller;
        [SerializeField] GameStateManager gameStateManager;
        [SerializeField] PlayerFSM fsm;

        [SerializeField] DoorCutsceneAnimation defaultAnim;

        [HideInInspector] public int animation;

        public override void Begin(Door target)
        {
            if (animation == 1)
            {
                gameStateManager.SwitchState(GameState.Cutscene);
                miller.gameObject.SetActive(true);
                //play miller jump scare
                //play camera turn animation

            }
            else
            {
                defaultAnim.StartAnimation(target, time);
            }
        }

        public override void Animate(Door target)
        {
        }

        public override void End(Door target)
        {
            if (animation == 1)
            {
                gameStateManager.SwitchToPrevious();
                miller.StartFollowing(fsm.transform);

            }

        }
    }
}
