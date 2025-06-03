using UnityEngine;
using System.Collections;
using amogus;

public class MillerCutsceneTrigger : TimelinePlayerTrigger
{
    [Header("Cutscene Object")]
    public GameObject objectToSpawn;

    [Header("Cutscene Camera")]
    public Camera cutsceneCamera;

    //[SerializeField] GhostChaseTrigger _loreRoomDoor;

    private bool cutscenePlaying = false;
    private GameObject spawnedObject;

    public void SwitchToCutsceneCam()
    {
        cutsceneCamera.enabled = true;
        
        objectToSpawn.SetActive(true);
    }

    public void SwitchFromCutsceneCame()
    {
        cutsceneCamera.enabled = false;
        objectToSpawn.SetActive(false);
        //_loreRoomDoor.CanUnlcok = true;
        
        //_loreRoomDoor.Unlock();
        //_loreRoomDoor.Trigger();

    }

}