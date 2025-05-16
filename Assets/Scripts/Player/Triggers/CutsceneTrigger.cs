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
    public abstract class CutsceneTrigger<Tr, Ex> : MonoBehaviour 
        where Tr : MonoBehaviour 
        where Ex : MonoBehaviour
    {
        public bool continiousDetection = false;
        public bool disableControls = true;
        [SerializeField] protected Ex target;
        public Tr triggerObject { get; protected set; }
        [SerializeField] List<Tr> shouldCheck = new();
        public virtual Predicate<Tr> Predicate => (Tr target) => true;
        public abstract ScriptedAnimation<Ex> Cutscene { get; }
        public virtual void StartCutscene(Ex target)
        {
            Cutscene.StartAnimation(target);
        }

        //Trigger is a part of fixed update, so input is not kinda working good
        private void OnTriggerEnter(Collider other)
        {
            var triggerObjectCandidate = other.GetComponent<Tr>();
            if (triggerObjectCandidate == null) return;

            if (!continiousDetection)
            {
                CheckTrigger(triggerObjectCandidate);
                return;
            }
            if (shouldCheck.Contains(triggerObjectCandidate)) return;
            shouldCheck.Add(triggerObjectCandidate);

        }

        private void OnTriggerStay(Collider other)
        {
        }

        private void OnTriggerExit(Collider other)
        {
            if (!continiousDetection) return;

            var triggerObjectCandidate = other.GetComponent<Tr>();
            if (triggerObjectCandidate == null) return;

            if (!shouldCheck.Contains(triggerObjectCandidate)) return;
            shouldCheck.Remove(triggerObjectCandidate);
        }

        private void Update()
        {
            Debug.Log(shouldCheck.Count);
            for (int i=shouldCheck.Count - 1; i >= 0; i--)
            {
                var other = shouldCheck[i];
                if (other != null)
                {
                    CheckTrigger(other);
                }
            }
        }
        protected virtual void CheckTrigger(Tr other)
        {
            if (other == null) return;

            if (Predicate(other) && enabled && !Cutscene.isRunningOn(target))
            {
                triggerObject = other;
                StartCutscene(target);
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
        protected override void CheckTrigger(T other)
        {
            target = other.GetComponent<T>();
            if (target == null) return;

            if (Predicate(target) && enabled && !Cutscene.isRunningOn(target))
            {
                triggerObject = target;
                StartCutscene(target);
            }
        }
    }
}