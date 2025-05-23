using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.GraphicsBuffer;

namespace amogus
{
    [Serializable]
    /// <summary>
    /// A general class for handling animations
    /// </summary>
    public abstract class ScriptedAnimation<T> : IPausable where T : MonoBehaviour
    {
        public float time { get; private set; }
        public float totalDuration;
        public float time01 => time / totalDuration;
        public float time10 => 1 - time01;

        public bool pause;
        public UnityEvent OnEnd;

        [SerializeField] List<T> users = new List<T>();
        public bool isRunningOn(T executor)
        {
            return users.Contains(executor);
        }
        public bool isRunningOnAny()
        {
            return users.Count > 0;
        }

        public virtual void StartAnimation(T executor, float time = 0)
        {
            this.time = time;
            Begin(executor);
            executor.StartCoroutine(Step(executor));
        }

        public void Terminate(T executor)
        {
            executor.StopCoroutine(Step(executor));
            users?.Remove(executor);

            OnEnd?.Invoke();
            End(executor);
            OnEnd.RemoveAllListeners();
        }
        public void Pause()
        {
            pause = true;
        }
        public void Resume()
        {
            pause = false;
        }
        IEnumerator Step(T target)
        {
            users.Add(target);
            var waitForEndOfFrame = new WaitForEndOfFrame();
            while (time < totalDuration)
            {
                if (pause)
                {
                    yield return waitForEndOfFrame;
                }
                time += Time.deltaTime;
                Animate(target);
                yield return waitForEndOfFrame;
            }
            EndAnimation(target);
            yield break;
        }

        void EndAnimation(T target)
        {
            OnEnd?.Invoke();
            Debug.Log("Scripted Animation: Animation Ended");
            time = totalDuration;
            End(target);
            OnEnd.RemoveAllListeners();
            users?.Remove(target);
        }
        public abstract void Animate(T target);
        public abstract void Begin(T target);
        public abstract void End(T target);
    }
}
