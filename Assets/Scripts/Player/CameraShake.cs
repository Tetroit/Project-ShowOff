
using amogus;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class CameraWalkingShake : MonoBehaviour
{
    public enum State
    {
        WALKING = 0,
        CROUCHING = 1,
        SPRINTING = 2,
        LADDER = 3,
    }

    Vector3 offset;
    float rotationOffset;
    public float frequency => currentShakeState.frequency;
    public float rotationIntensity => currentShakeState.rotationIntensity;
    float _cooldown;

    public bool soundRotation = true;
    public AnimationCurve XShake => currentShakeState.XShake;
    public AnimationCurve YShake => currentShakeState.YShake;
    public bool resetOnDoingNothing => currentShakeState.resetOnDoingNothing;
    public PlayerFSM controls;

    [SerializeField] List<CameraShakeState> states;
    State currentShakeStateID = 0;
    public CameraShakeState currentShakeState => states[(int)currentShakeStateID];
    //public AudioSource stepSource;
    //public AudioClip[] stepSounds;

    bool _rightSide;
    float _amplitude = 0;
    //int _currentClipID = 0;
    

    void Start()
    {
        //if (stepSource == null)
        //{
        //    stepSource = GetComponent<AudioSource>();
        //    if (stepSource == null)
        //    {
        //        stepSource = gameObject.AddComponent<AudioSource>();
        //        stepSource.outputAudioMixerGroup = Settings.main.GetMixerChannel(Settings.PLAYER_GROUP);
        //    }
        //}
    }

    // Update is called once per frame
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
                    AudioManager.instance.PlayOneShot(FMODEvents.instance.footSteps, transform.position);
                }

                //if (stepSource != null && stepSounds.Length != 0)
                //{
                //    if (soundRotation)
                //    {
                //        _currentClipID++;
                //        if (_currentClipID == stepSounds.Length) _currentClipID = 0;
                //        stepSource.clip = stepSounds[_currentClipID];
                //    }
                //    else
                //    {
                //        stepSource.clip = stepSounds[Random.Range(0, stepSounds.Length)];
                //    }
                //    stepSource.Play();
                //}
            }
        }
        else
        {
            //if (resetOnDoingNothing || controls.isMoving)
                _cooldown += Time.deltaTime;
        }
        float facY = _cooldown / frequency;
        float facX = facY;

        offset = new Vector3(_rightSide ? XShake.Evaluate(facX) : -XShake.Evaluate(facX), YShake.Evaluate(facY), 0f);
        offset.x *= _amplitude;
        rotationOffset = -rotationIntensity * offset.x;

        transform.localEulerAngles = new Vector3(0f, 0f, rotationOffset);
        transform.localPosition = offset;

    }

    public void ChangeState(State ID)
    {
        _cooldown = _cooldown / currentShakeState.frequency * states[(int)ID].frequency;
        currentShakeStateID = ID;
    }
}
