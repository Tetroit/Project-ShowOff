using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace amogus
{
    [Serializable]
    /// <summary>
    /// A general class for handling animations
    /// </summary>
    public abstract class ScriptedAnimation<T> : ScriptableObject where T : MonoBehaviour
    {
        public float time { get; private set; }
        public float totalDuration;
        public float time01 => time / totalDuration;
        public float time10 => 1 - time01;

        public Action OnEnd;

        public virtual void StartAnimation(T executor, float time = 0)
        {
            this.time = time;
            Begin(executor);
            executor.StartCoroutine(Step(executor));
        }

        public void Terminate(T executor)
        {
            executor.StopCoroutine(Step(executor));
        }
        IEnumerator Step(T target)
        {
            while (time < totalDuration)
            {
                Debug.Log($"FAC: {time01}");
                time += Time.deltaTime;
                Animate(target);
                yield return new WaitForEndOfFrame();
            }
            Debug.Log("SA: Animation Ended");
            time = totalDuration;
            End(target);
            OnEnd?.Invoke();
            OnEnd = null;
            yield break;
        }

        public abstract void Animate(T target);
        public abstract void Begin(T target);
        public abstract void End(T target);
    }
}
