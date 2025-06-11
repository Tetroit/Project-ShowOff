using FMODUnity;
using UnityEngine;

public class PlayAudio : MonoBehaviour
{
    [SerializeField] private EventReference sound;

    public void PlaySound()
    {
        AudioManager.instance.PlayOneShot(sound, transform.position);
    }
}
