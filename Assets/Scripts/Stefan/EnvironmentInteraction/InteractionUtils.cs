using System;
using System.Collections;
using UnityEngine;

public static class InteractionUtils 
{
    public static Coroutine RunCoroutineWithCallback(this MonoBehaviour monoBehaviour, IEnumerator routine, Action callback)
    {
        return monoBehaviour.StartCoroutine(Wrapped());

        IEnumerator Wrapped()
        {
            yield return monoBehaviour.StartCoroutine(routine);
            callback?.Invoke();
        }
    }
}
