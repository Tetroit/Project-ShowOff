using System;
using UnityEngine;


namespace amogus
{
    public class ChostChaseTrigger : CutsceneTrigger<PlayerFSM, MillerController>
    {
        [SerializeField] private ChostChaseAnimation cutscene;
        public string unlockCode;
        public bool isLocked = false;

        public override ScriptedAnimation<MillerController> Cutscene => cutscene ;
        public override Predicate<PlayerFSM> Predicate => (PlayerFSM player) =>
        {
            return true;
        };
    }

    [Serializable]
    public class ChostChaseAnimation : ScriptedAnimation<MillerController>
    {
        [SerializeField] PlayerFSM player;
        [SerializeField] GameStateManager gameStateManager;


        public override void Begin(MillerController target)
        {
            target.gameObject.SetActive(true);
            gameStateManager.SwitchState(GameState.Cutscene);
        }

        public override void Animate(MillerController target)
        {

        }

        public override void End(MillerController target)
        {
            target.gameObject.SetActive(false);
            gameStateManager.SwitchToPrevious();
        }
    }
}
