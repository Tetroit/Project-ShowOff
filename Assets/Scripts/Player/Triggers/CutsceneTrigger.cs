using System;
using System.Collections.Generic;
using UnityEngine;

namespace amogus
{
    [RequireComponent(typeof(Collider))]
    public abstract class CutsceneTrigger : MonoBehaviour
    {
        public abstract PlayerAnimation Cutscene { get; }

        public abstract void TransferData(PlayerController controller);

        public virtual void StartCutscene(PlayerFSM player)
        {
            Cutscene.StartAnimation(player);
        }

        //protected virtual void OnDrawGizmos()
        //{
        //    Gizmos.color = Color.red;
        //    Gizmos.DrawWireSphere(transform.position, 0.5f);
        //}
    }
}