using FMODUnity;
using UnityEngine;
using UnityEngine.Events;

public class PlayAudioOnDistanceTraveled : MonoBehaviour
{
    public float DistanceUntilStep = 1f;
    [SerializeField] EventReference _sound;
    Vector3 _lastLocation;

    [field: SerializeField] public UnityEvent OnStep { get; private set; }

    void Update()
    {
        if (Vector3.Distance(_lastLocation, transform.position) > DistanceUntilStep)
        {
            _lastLocation = transform.position;
            AudioManager.instance.PlayOneShot(_sound, transform.position);
            OnStep?.Invoke();
        }
    }
}
