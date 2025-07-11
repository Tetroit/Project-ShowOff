using UnityEngine;
using amogus;

public class MillerCutsceneTrigger : TimelinePlayerTrigger
{
    [Header("Cutscene Object")]
    public GameObject objectToSpawn;

    [Header("Cutscene Camera")]
    public Camera cutsceneCamera;

    public void SwitchToCutsceneCam()
    {
        cutsceneCamera.enabled = true;
        
        objectToSpawn.SetActive(true);
    }

    public void SwitchFromCutsceneCame()
    {
        cutsceneCamera.enabled = false;
        objectToSpawn.SetActive(false);
    }

}