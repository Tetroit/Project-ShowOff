using amogus;
using System;
using UnityEngine;
public class SimplePlayerCutscene : TimelineTrigger<PlayerFSM>
{
    [SerializeField] GameStateManager _gameStateManager;

    public override Predicate<PlayerFSM> Predicate => (PlayerFSM player) => {
        return director.state != UnityEngine.Playables.PlayState.Playing;
    };

    public override void Trigger()
    {
        base.Trigger();

        _gameStateManager.SwitchState(GameState.Cutscene);

    }

    protected override void AnimationEnd()
    {
        _gameStateManager.SwitchToPrevious();
    }
}
