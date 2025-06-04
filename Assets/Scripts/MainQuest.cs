using amogus;
using UnityEngine;
using UnityEngine.Events;

public class MainQuest : MonoBehaviour
{
    [System.Serializable]
    public enum QuestStep
    {
        Start = 0,
        FellThroughFloor = 1,
        FootstepsTriggered = 2,
        FootstepsDoorClosed = 3,
        GhostChaseStarted = 4,
        GhostChaseEnded = 5,
        SawMiller = 6,
    }
    public UnityEvent<QuestStep> OnNextGoal;
    [SerializeField] QuestStep _currentStep = 0;

    [SerializeField] DoorCutsceneTrigger LoreRoomDoor;
    [SerializeField] DoorCutsceneTrigger HallwayDoor;
    [SerializeField] DoorCutsceneTrigger OfficeDoor;

    private void Awake()
    {
        
    }
    public QuestStep currentStep => _currentStep;
    public void Advance()
    {
        _currentStep++;
        OnNextStep();
    }
    public void AdvanceTo(QuestStep step)
    {
        _currentStep = step;
        OnNextStep();
    }
    public void AdvanceTo(int step)
    {
        _currentStep = (QuestStep)step;
        OnNextStep();
    }

    public void OnNextStep()
    {
        Debug.Log("Quest update: " + _currentStep, this);
        OnNextGoal?.Invoke(_currentStep);
        switch (_currentStep)
        {
            case QuestStep.GhostChaseStarted:
                var gcd = HallwayDoor.GetComponent<GhostChaseDeactivator>();
                gcd.enabled = true;
                gcd.Hook();
                break;
            case QuestStep.GhostChaseEnded:
                HallwayDoor.Lock();
                OfficeDoor.Unlock();
                break;
        }
    }


    public bool Validate()
    {
        return true;
    }
}
