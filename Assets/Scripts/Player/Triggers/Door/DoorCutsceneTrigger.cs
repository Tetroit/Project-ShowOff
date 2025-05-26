using System;
using UnityEngine;

namespace amogus
{
    public class DoorCutsceneTrigger : DoorTrigger
    {
        [SerializeField] private DoorCutsceneAnimation cutscene;

        public override ScriptedAnimation<Door> Cutscene => cutscene;
        public override Predicate<PlayerFSM> Predicate => (PlayerFSM player) => {
            return !isLocked;
        };

        public override void Unlock()
        {
            isLocked = false;
        }
    }
}
