using amogus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraWalkingShake : MonoBehaviour
{
    public enum State
    {
        WALKING = 0,
        CROUCHING = 1,
        SPRINTING = 2,
        LADDER = 3,
    }
    
    public float frequency => currentShakeState.frequency;
    public float rotationIntensity => currentShakeState.rotationIntensity;
    public AnimationCurve XShake => currentShakeState.XShake;
    public AnimationCurve YShake => currentShakeState.YShake;
    public bool resetOnDoingNothing => currentShakeState.resetOnDoingNothing;
    public CameraShakeState currentShakeState => states[(int)currentShakeStateID];

    public bool soundRotation = true;
    public PlayerFSM controls;

    [SerializeField] List<CameraShakeState> states;
    [SerializeField] private FootstepsPlayer _footstepsPlayer;

    State currentShakeStateID = 0;
    bool _rightSide;
    float _amplitude = 0;
    Vector3 offset;
    float rotationOffset;
    float _cooldown;


    Stack<Vector3> _oneShotOffsets = new();

    void Update()
    {
        if (controls.isMoving) _amplitude = Mathf.Lerp(_amplitude, 1, 0.01f);
        else if (resetOnDoingNothing) _amplitude = Mathf.Lerp(_amplitude, 0, 0.01f);
        if (_cooldown >= frequency)
        {
            if (controls.isMoving)
            {
                _cooldown -= frequency;
                _rightSide = !_rightSide;
                if(FMODEvents.instance != null)
                {
                    if(currentShakeStateID == State.LADDER)
                    {
                        _footstepsPlayer.PlayLadderSound();
                    }
                    else
                    {
                        _footstepsPlayer.PlayFootstepSound();
                    }
                }
            }
        }
        else
        {
            _cooldown += Time.deltaTime;
        }
        float facY = _cooldown / frequency;
        float facX = facY;

        offset = new Vector3(_rightSide ? XShake.Evaluate(facX) : -XShake.Evaluate(facX), YShake.Evaluate(facY), 0f);
        offset.x *= _amplitude;
        rotationOffset = -rotationIntensity * offset.x;

        transform.localEulerAngles = new Vector3(0f, 0f, rotationOffset);
        Vector3 oneShotOffsets = new();
        while(_oneShotOffsets.Count > 0)
        {
            oneShotOffsets += _oneShotOffsets.Pop();
        }


        transform.localPosition = offset + oneShotOffsets;
    }

    public void Shake(float time, float scale, float frequency)
    {
        if(gameObject.activeInHierarchy)
            StartCoroutine(OneShotShake(time, scale, frequency));
    }

    IEnumerator OneShotShake(float time, float scale, float frequency)
    {
        var wait = new WaitForSeconds(frequency);
        while(time > 0)
        {
            var oneShotOffset = new Vector3(UnityEngine.Random.Range(0, 1f), UnityEngine.Random.Range(0, 1f)) * scale;
            _oneShotOffsets.Push(oneShotOffset);
            yield return wait;
            time -= Time.deltaTime;
        }
    }


    public void ChangeState(State ID)
    {
        _cooldown = _cooldown / currentShakeState.frequency * states[(int)ID].frequency;
        currentShakeStateID = ID;
    }
}
