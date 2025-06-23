using UnityEngine;

public class ArmGone : MonoBehaviour
{
    public void Deactivate()
    { 
        Debug.Log("Deactivating " + gameObject.name + " via Timeline signal.");

        gameObject.SetActive(false);
    }
}