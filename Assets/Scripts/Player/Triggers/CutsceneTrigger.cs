using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace amogus
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T">Trigger object type</typeparam>
    /// <typeparam name="U">Executor object type</typeparam>
    [RequireComponent(typeof(Collider))]
    public abstract class CutsceneTrigger<Tr, Ex> : SimpleTrigger<Tr> 
        where Tr : MonoBehaviour 
        where Ex : MonoBehaviour
    {
        public bool disableControls = true;
        [SerializeField] protected Ex target;   
        public abstract ScriptedAnimation<Ex> Cutscene { get; }
        public override void Trigger()
        {
            Cutscene.StartAnimation(target);
        }
        protected override void TryTrigger(Tr other)
        {
            Debug.Log("Tried activating " + this);
            if (other == null) return;

            if (enabled && !Cutscene.isRunningOn(target))
            {
                Debug.Log(Predicate(other));
                if (Predicate(other))
                {
                    triggerObject = other;
                    Trigger();
                    Debug.Log("Activated " + this);
                }
            }
        }
        //protected virtual void OnDrawGizmos()
        //{
        //    Gizmos.color = Color.red;
        //    Gizmos.DrawWireSphere(transform.position, 0.5f);
        //}
    }

    public abstract class CutsceneTrigger<T> : CutsceneTrigger<T, T> where T : MonoBehaviour 
    {
        protected override void TryTrigger(T other)
        {
            target = other.GetComponent<T>();
            if (target == null) return;

            if (Predicate(target) && enabled && !Cutscene.isRunningOn(target))
            {
                triggerObject = target;
                Trigger();
            }
        }
    }
}