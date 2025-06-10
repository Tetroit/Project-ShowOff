
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System;


public enum TriggerType
{
    ENTER = 0,
    LEAVE = 1,
    PRESS_KEY = 2
}

[RequireComponent(typeof(Collider))]
public abstract class SimpleTrigger<TargetType> : MonoBehaviour
    where TargetType : Component
{
    public virtual Predicate<TargetType> Predicate => (TargetType target) => true;

    [SerializeField] TriggerType triggerType;
    public string action = "Interact";

    [Header("Events")]
    [SerializeField] public UnityEvent OnTrigger;


    protected List<TargetType> targetsInQuestion = new();
    public TargetType triggerObject { get; protected set; }

    protected virtual void OnTriggerEnter(Collider other)
    {
        var triggerObjectCandidate = other.GetComponent<TargetType>();
        if (triggerObjectCandidate == null) return;

        if (!targetsInQuestion.Contains(triggerObjectCandidate))
            targetsInQuestion.Add(triggerObjectCandidate);

        if (triggerType == TriggerType.ENTER)
        {
            TryTrigger(triggerObjectCandidate);
            return;
        }
    }

    protected virtual void OnTriggerStay(Collider other)
    {
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        var triggerObjectCandidate = other.GetComponent<TargetType>();
        if (triggerObjectCandidate == null) return;

        if (targetsInQuestion.Contains(triggerObjectCandidate))
            targetsInQuestion.Remove(triggerObjectCandidate);

        if (triggerType == TriggerType.LEAVE)
        {
            TryTrigger(triggerObjectCandidate);
            return;
        }

    }

    protected virtual void Update()
    {
        if (triggerType == TriggerType.PRESS_KEY && InputSystem.actions.FindActionMap("Player").FindAction(action).WasPressedThisFrame())
        {
            for (int i = targetsInQuestion.Count - 1; i >= 0; i--)
            {
                var other = targetsInQuestion[i];
                if (other != null)
                {
                    TryTrigger(other);
                }
            }
        }
    }

    protected virtual bool NullHandling(TargetType target)
    {
        if (target == null)
        {
            Debug.LogError("Target object was null", this);
            return false;
        }
        return true;
    }
    protected virtual void TryTrigger(TargetType other)
    {
        if (!NullHandling(other)) return;

        if (Predicate(other) && enabled)
        {
            Debug.Log("Triggered", this);
            triggerObject = other;
            Trigger();
            OnTrigger?.Invoke();

        }
    }
    public abstract void Trigger();
}
