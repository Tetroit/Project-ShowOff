using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace amogus
{
    [CreateAssetMenu(menuName = "Animations/PlayerAnimation")]
    public class PlayerAnimation : ScriptedAnimation<PlayerFSM>
    {
        public override void Animate(PlayerFSM target)
        {
        }

        public override void Begin(PlayerFSM target)
        {
        }

        public override void End(PlayerFSM target)
        {
        }
    }
}
