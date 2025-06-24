using FMODUnity;
using UnityEngine;

public class PlayOnEnable : MonoBehaviour
{
    [SerializeField] private EventReference sound;
    private void OnEnable()
    {
        if(AudioManager.instance != null)
        {
            AudioManager.instance.PlayOneShot(sound, transform.position);
        }
    }
}
