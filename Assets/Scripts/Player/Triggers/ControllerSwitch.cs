using amogus;
using Unity.VisualScripting;
using UnityEngine;
using System;



namespace amogus
{
    [RequireComponent(typeof(Collider))]
    public abstract class ControllerSwitch : MonoBehaviour
    {
        public abstract PlayerFSM.ControllerType FromType { get; }
        public abstract PlayerFSM.ControllerType ToType { get; }
        public abstract PlayerAnimation ForwardTransitionBase { get; }
        public abstract PlayerAnimation BackwardTransitionBase { get; }
        public bool useForwardAnimation;
        public bool useBackwardAnimation;
        public Action OnTrigger;
    }

    public abstract class ControllerSwitch<T,U> : ControllerSwitch 
        where T : PlayerAnimation 
        where U : PlayerAnimation
    {
        public T ForwardTransition => (T)ForwardTransitionBase;
        public U BackwardTransition => (U)ForwardTransitionBase;
    }
}
