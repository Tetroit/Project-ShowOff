using UnityEngine;
using UnityEngine.Events;

public class MainQuest : MonoBehaviour
{
    public enum QuestStep
    {
        Start = 0,
        FellThroughFloor = 1,
        FootstepsTriggered = 2,
        FootstepsDoorClosed = 3,
        GhostChaseStarted = 4,
        GhostChaseEnded = 5,
    }
    UnityEvent<QuestStep> OnNextGoal;
    [SerializeField] QuestStep _currentStep = 0;

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

    public void OnNextStep()
    {
        OnNextGoal?.Invoke(_currentStep);
        switch (_currentStep)
        {
        }
    }
}
