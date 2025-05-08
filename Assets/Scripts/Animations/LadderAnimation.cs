using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace amogus
{
    [CreateAssetMenu(menuName = "Animations/LadderTransition")]
    public class LadderAnimation : PlayerAnimation
    {
        Vector3 startPos;
        Quaternion startRot;
        [SerializeField] Vector3 targetPos;
        [SerializeField] Vector3 targetRot;
        public override void Begin(PlayerFSM target)
        {
            startPos = target.transform.position;
            startRot = target.transform.rotation;
        }
        public override void Animate(PlayerFSM target)
        {
            var q = Quaternion.Slerp(startRot, Quaternion.Euler(targetRot), Mathf.Min(time01 * 2, 1));
            Debug.Log(q);
            target.transform.SetPositionAndRotation(Vector3.Lerp(startPos, targetPos, time01), q);
        }


        public override void End(PlayerFSM target)
        {
            target.ResetCamera();
        }
    }
}
