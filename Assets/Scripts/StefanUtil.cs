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
}