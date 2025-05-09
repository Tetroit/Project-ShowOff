using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace amogus
{
    [Serializable]
    public class LadderAnimation : PlayerAnimation
    {
        Vector3 startPos;
        Quaternion startRot;
        [SerializeField] Vector3 targetPos;
        [SerializeField] Vector3 targetRot;
        [SerializeField]
        Ladder ladder;
        public override void Begin(PlayerFSM target)
        {
            startPos = target.transform.position;
            startRot = target.transform.rotation;
        }
        public override void Animate(PlayerFSM target)
        {
            var q = Quaternion.Slerp(startRot, Quaternion.Euler(targetRot), time01);
            target.transform.SetPositionAndRotation(Vector3.Lerp(startPos, targetPos, time01), q);
        }


        public override void End(PlayerFSM target)
        {
            target.ResetCamera();
        }
    }
}
