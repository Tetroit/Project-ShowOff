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

    [SerializeField] bool playAllStepsTillCurrentStepOnAwake = true;
    [SerializeField] TimelinePlayerTrigger _floorFallTrigger;
    [SerializeField] DoorCutsceneTrigger LoreRoomDoor;
    [SerializeField] DoorCutsceneTrigger HallwayDoor;
    [SerializeField] DoorCutsceneTrigger OfficeDoor;
    [SerializeField] PlayerTrigger footstepTriggerStart;
    [SerializeField] PlayerTrigger footstepTriggerEnd;
    [SerializeField] FollowingSteps footsteps;
    [SerializeField] QuickTimeEvent doorCloseQTE;
    [SerializeField] FearValue fearValue;

    private void Awake()
    {
        if (playAllStepsTillCurrentStepOnAwake)
        {
            FastForward((int)_currentStep);
        }
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
        //_currentStep = (QuestStep)step;
        //OnNextStep();
        FastForward(step);
    }

    public void OnNextStep(bool instant = false)
    {
        Debug.Log("Quest update: " + _currentStep, this);
        OnNextGoal?.Invoke(_currentStep);
        switch (_currentStep)
        {
            //case QuestStep.Start:
            //    LoreRoomDoor.Unlock();
            //    break;
            case QuestStep.FellThroughFloor:
                if (_floorFallTrigger == null) Debug.LogWarning("Go to Main quest and reference floor fall trigger");
                else _floorFallTrigger.gameObject.SetActive(false); 
                break;
            case QuestStep.FootstepsTriggered:
                footstepTriggerStart.enabled = false;
                footstepTriggerEnd.enabled = true;
                footsteps.StartFollowing();

                HallwayDoor.enabled = false;
                break;
            case QuestStep.FootstepsDoorClosed:
                footstepTriggerEnd.enabled = false;
                footsteps.EndFollowing();
                
                HallwayDoor.enabled = true;
                if (instant)
                    HallwayDoor.CloseInstant();
                else
                    HallwayDoor.SetExternally(false);
                break;
            case QuestStep.GhostChaseStarted:
                {
                    var gcd = doorCloseQTE.GetComponent<GhostChaseDeactivator>();
                    gcd.enabled = true;
                    //gcd.Hook();
                }
                {
                    var gca = LoreRoomDoor.GetComponent<GhostChaseActivator>();
                    gca.enabled = false;
                }
                {
                    var pltr = doorCloseQTE.GetComponent<TimelinePlayerTrigger>();
                    pltr.enabled = true;
                }

                if (fearValue != null)
                {
                    fearValue.enabled = true;
                }

                if (!instant)
                {
                    if (HallwayDoor.isOpen)
                    {
                        HallwayDoor.enabled = false;
                    }
                    else
                    {
                        HallwayDoor.TriggerOnceAfterAnimation(() =>
                        {
                            HallwayDoor.enabled = false;
                        });
                        HallwayDoor.TriggerExternally();
                    }
                }
                else
                {
                    HallwayDoor.enabled = false;
                    HallwayDoor.OpenInstant();
                }
                break;
            case QuestStep.GhostChaseEnded:

                HallwayDoor.enabled = true;

                if (fearValue != null)
                {
                    if (instant)
                        fearValue.enabled = false;
                    else
                        fearValue.SmoothlyDisable(4f);
                }

                if (instant)
                    HallwayDoor.CloseInstant();
                else
                    HallwayDoor.SetExternally(false);

                HallwayDoor.Lock();
                OfficeDoor.Unlock();

                if (instant)
                    OfficeDoor.OpenInstant();
                else
                    OfficeDoor.SetExternally(true);
                {
                    var pltr = doorCloseQTE.GetComponent<TimelinePlayerTrigger>();
                    pltr.enabled = false;
                }
                break;

            case QuestStep.SawMiller:

                HallwayDoor.Unlock();
                LoreRoomDoor.Unlock();

                if (instant)
                    LoreRoomDoor.OpenInstant();
                else
                    LoreRoomDoor.SetExternally(true);

                break;
        }
    }

    public void FastForward(int step)
    {
        for (; (int)_currentStep < step; )
        {
            _currentStep++;
            if ((int)_currentStep == step)
                OnNextStep();
            else
                OnNextStep(true);
        }
    }

    public bool Validate()
    {
        return true;
    }
}
