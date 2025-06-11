using FMOD.Studio;
using FMODUnity;
using System.Diagnostics.Tracing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class VolumeSliders : MonoBehaviour , IPointerUpHandler
{
    [SerializeField] private string Usedbus;
    [SerializeField] private EventReference testSound;
    private Bus bus;

    private void Start()
    {
        bus = RuntimeManager.GetBus(Usedbus); 

    }

    public void ChangeVolume(float volume)
    {
         bus.setVolume(volume);
         
    }

    public void OnPointerUp(PointerEventData eventData)
    {
            AudioManager.instance.PlayOneShot(testSound, transform.position);
    }
}

