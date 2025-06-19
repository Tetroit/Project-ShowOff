using FMODUnity;
using UnityEngine;

public class PlayOnEnable : MonoBehaviour
{
    [SerializeField] private EventReference sound;
    private void OnEnable()
    {
        AudioManager.instance.PlayOneShot(sound, transform.position);
    }
}
