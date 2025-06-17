using FMOD;
using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance { get; private set; }
    private Bus masterBus;
    private List<EventInstance> eventInstances;

    public EventInstance ambienceEventInstance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        instance = this;

        eventInstances = new List<EventInstance>();
    }

    private void Start()
    {
        masterBus = RuntimeManager.GetBus("bus:/");
        InitializeAmbience(FMODEvents.instance.ambience);
    }


    public void PlayOneShot(EventReference sound, Vector3 sourcePos)
    {
        RuntimeManager.PlayOneShot(sound, sourcePos);
    }

    private void InitializeAmbience(EventReference ambienceEventReference)
    {
        ambienceEventInstance = CreateInstance(ambienceEventReference);
        ambienceEventInstance.set3DAttributes(RuntimeUtils.To3DAttributes(Vector3.zero));
        ambienceEventInstance.start();
    }

    public void SetAmbienceByParameter(string parameterName, float parameterValue)
    {
        var result = ambienceEventInstance.setParameterByName(parameterName, parameterValue);
        if(result != RESULT.OK)
        {
            RuntimeManager.StudioSystem.setParameterByName(parameterName, parameterValue);
        }
    }

    public void SetAmbienceArea(AmbienceArea area)
    {
        ambienceEventInstance.setParameterByName("AreaMusic", (float)area);
    }

    public EventInstance CreateInstance(EventReference eventReference)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
        eventInstances.Add(eventInstance);
        return eventInstance;
    }

    public void PauseAllAudio()
    { 
            masterBus.setPaused(true);
    }

    public void UnPauseAllAudio()
    {
            masterBus.setPaused(false);
    }

    public void CleanUp()
    {
        foreach (var eventInstance in eventInstances)
        {
            eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            eventInstance.release();
        }

        masterBus.stopAllEvents(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }

    void OnDestroy()
    {
        CleanUp();

    }
}
