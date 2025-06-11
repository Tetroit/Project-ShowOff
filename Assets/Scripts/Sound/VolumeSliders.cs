using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class VolumeSliders : MonoBehaviour
{
    [SerializeField] private string Usedbus;
    private Bus bus;

    private void Start()
    {
        bus = RuntimeManager.GetBus(Usedbus); 

    }

    public void ChangeVolume(float volume)
    {
         bus.setVolume(volume);
        Debug.Log(volume);
    }
}
