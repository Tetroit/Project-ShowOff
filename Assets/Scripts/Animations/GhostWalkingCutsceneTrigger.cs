using UnityEngine;
using System.Collections;
using amogus;

public class MillerCutsceneTrigger : TimelinePlayerTrigger
{
    [Header("Cutscene Object")]
    public GameObject objectToSpawn;

    [Header("Cutscene Camera")]
    public Camera cutsceneCamera;

    private bool cutscenePlaying = false;
    private GameObject spawnedObject;

    public void SwitchToCutsceneCam()
    {
        cutsceneCamera.enabled = true;
    }

    public void SwitchFromCutsceneCame()
    {
        cutsceneCamera.enabled = false;
    }
    
}