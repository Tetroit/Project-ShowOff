using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

public class QuickTimeEvent : MonoBehaviour
{

    [Header("Time")]
    public float totalTime;
    float time;
    bool isReading;

    [Header("Action")]
    public bool success { get; private set; }

    public string actionName;
    public int pressCount;
    int pressCountCurrent;

    [Header("Events")]
    public UnityEvent OnStartReading;
    public UnityEvent OnStopReading;
    public UnityEvent OnSuccess;
    public UnityEvent OnFail;
    public UnityEvent OnActionPress;

    public QuickTimeEventIndicator indicator;

    public bool Check()
    {
        if (pressCountCurrent >= pressCount)
        {
            success = true;
        }
        else
        {
            success = false;
        }
        return success;
    }

    public void StartReading()
    {
        OnStartReading?.Invoke();
        InputSystem.actions.FindActionMap("Player").FindAction(actionName).started += OnPress;
        isReading = true;
        pressCountCurrent = 0;
        Debug.Log("QuickTimeEvent activated");
        if (indicator != null)
        {
            indicator.pressCount = pressCount;
            indicator.gameObject.SetActive(true);
        }
    }

    public void Update()
    {
        if (isReading)
        {
            time += Time.deltaTime;
            if (time >= totalTime || pressCountCurrent >= pressCount)
            {
                isReading = false;
                StopReading();
                time = 0f;
            }
        }
    }
    public void StopReading()
    {
        isReading = false;
        Check();
        OnStopReading?.Invoke();
        if (success)
        {
            indicator.SetSuccess();
            OnSuccess?.Invoke();
        }
        else
        {
            indicator.SetFail();
            OnFail?.Invoke();
        }

        if (indicator != null)
        {
            indicator.SmoothDisable();
        }
    }

    public void OnPress(InputAction.CallbackContext callback)
    {
        OnActionPress?.Invoke();
        pressCountCurrent++;
    }
}
