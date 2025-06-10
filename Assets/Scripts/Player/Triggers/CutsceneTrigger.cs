using System;
using UnityEngine;

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

        [Header("Animations")]
        public abstract ScriptedAnimation<Ex> Cutscene { get; }

        public override Predicate<Tr> Predicate => (Tr other) =>
        {
            if (Cutscene.isRunningOn(target)) return false; 
            return true;
        };
        public override void Trigger()
        {
            Cutscene.StartAnimation(target);
        }
        //protected virtual void OnDrawGizmos()
        //{
        //    Gizmos.color = Color.red;
        //    Gizmos.DrawWireSphere(transform.position, 0.5f);
        //}
    }

    public abstract class CutsceneTrigger<T> : CutsceneTrigger<T, T> where T : MonoBehaviour 
    {
        public override Predicate<T> Predicate => (T other) =>
        {
            if (Cutscene.isRunningOn(target)) return false;
            return true;
        };
    }
}