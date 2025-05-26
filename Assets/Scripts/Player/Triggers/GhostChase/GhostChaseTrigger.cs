using System;
using UnityEngine;


namespace amogus
{
    public class GhostChaseTrigger : DoorTrigger
    {
        [SerializeField] GhostChaseAnimation cutscene;

        public override ScriptedAnimation<Door> Cutscene => cutscene ;
        public override Predicate<PlayerFSM> Predicate => (PlayerFSM player) =>
        {
            return cutscene.animation == 1 || !isLocked;
        };

        public override void Unlock()
        {
            cutscene.animation++;

            if (cutscene.animation != 1) isLocked = false; 
        }
    }
}
