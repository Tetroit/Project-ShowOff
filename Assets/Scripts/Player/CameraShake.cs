
using amogus;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class CameraWalkingShake : MonoBehaviour
{
    Vector3 offset;
    float rotationOffset;
    public float frequency;
    float _cooldown;

    public bool soundRotation = true;
    public AnimationCurve XShake;
    public AnimationCurve YShake;
    public PlayerFSM controls;
    //public AudioSource stepSource;
    //public AudioClip[] stepSounds;

    public float rotationIntensity;
    bool _rightSide;
    float _amplitude = 0;
    int _currentClipID = 0;
    

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
        else _amplitude = Mathf.Lerp(_amplitude, 0, 0.01f);
        if (_cooldown >= frequency)
        {
            if (controls.isMoving)
            {
                _cooldown -= frequency;
                _rightSide = !_rightSide;
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
        if (_cooldown < frequency)
        {
            _cooldown += Time.deltaTime;
        }
        float facY = _cooldown / frequency;
        float facX = facY;

        offset = new Vector3((_rightSide) ? XShake.Evaluate(facX) : -XShake.Evaluate(facX), YShake.Evaluate(facY), 0f);
        offset.x *= _amplitude;
        rotationOffset = -rotationIntensity * offset.x;

        transform.localEulerAngles = new Vector3(0f, 0f, rotationOffset);
        transform.localPosition = offset;

    }
}
