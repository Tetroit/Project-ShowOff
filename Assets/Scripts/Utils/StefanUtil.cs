using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class StefanUtil
{
    public static Transform FindDeepChild(this Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name)
                return child;

            Transform result = child.FindDeepChild(name);
            if (result != null)
                return result;
        }
        return null;
    }

    public static int IndexMatch<T>(this IEnumerable<T> container, Predicate<T> predicate)
    {
        int i = 0;
        foreach (T item in container)
            if (predicate(item)) return i;
            else i++;
        return -1;
    }

    public static Coroutine RunCoroutineWithCallback(this MonoBehaviour monoBehaviour, IEnumerator routine, Action callback)
    {
        return monoBehaviour.StartCoroutine(Wrapped());

        IEnumerator Wrapped()
        {
            yield return monoBehaviour.StartCoroutine(routine);
            callback?.Invoke();
        }
    }

    public static void Test(this SelectDirection direction)
    {
        Debug.Log(direction.ToString());
    }

}