using FMODUnity;
using UnityEngine;

public class MoveAmbience : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        AudioManager.instance.ambienceEventInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
    }
}
