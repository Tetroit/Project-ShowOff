using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        instance = this;
    }

    public void PlayOneShot(EventReference sound, Vector3 sourcePos)
    {
        RuntimeManager.PlayOneShot(sound, sourcePos);
    }
}
