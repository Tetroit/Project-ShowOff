﻿using System;
using UnityEngine;
#pragma warning disable CS0162

namespace amogus
{
    [Serializable]
    public class LadderAnimation : PlayerAnimation
    {
        Vector3 startPos;
        Quaternion startRot;
        Vector3 targetPos;
        Quaternion targetRot;
        [HideInInspector] public Ladder ladder;
        [HideInInspector] public Ladder.EndType whichEnd;
        public override void Begin(PlayerFSM target)
        {
            startPos = target.transform.position;
            if (!Ladder.noRotation)
                startRot = target.transform.rotation;

            if (target.currentControllerID == PlayerFSM.ControllerType.FREE_MOVE)
            {
                targetPos = ladder.GetClosestPoint(startPos);
                if (!Ladder.noRotation)
                    targetRot = ladder.facing;
            }

            else if (target.currentControllerID == PlayerFSM.ControllerType.LADDER)
            {
                targetPos = ladder.GetEntryPoint(whichEnd);
                if (!Ladder.noRotation)
                    targetRot = ladder.GetEntryRotation(whichEnd);
            }
        }
        public override void Animate(PlayerFSM target)
        {
            if (Ladder.noRotation)
                target.transform.position = Vector3.Lerp(startPos, targetPos, time01);
            else
            {
                var q = Quaternion.Slerp(startRot, targetRot, time01);
                target.transform.SetPositionAndRotation(Vector3.Lerp(startPos, targetPos, time01), q);
            }
        }

        public override void End(PlayerFSM target)
        {
            if (!Ladder.noRotation)
                target.ReadCamera();
        }

        public void LazyInitialiseFromSwitch(LadderSwitch lSwitch, Ladder.EndType end)
        {
            whichEnd = end;
            ladder = lSwitch.ladder;
        }
    }
}
