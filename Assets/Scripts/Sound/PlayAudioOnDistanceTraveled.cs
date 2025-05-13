using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAudioOnDistanceTraveled : MonoBehaviour
{
    [SerializeField] private float DistanceUntilStep = 1f;
    private Vector3 lastLocation;

    // Update is called once per frame
    void Update()
    {

        if (Vector3.Distance(lastLocation, transform.position) > DistanceUntilStep)
        {
            lastLocation = transform.position;
            AudioManager.instance.PlayOneShot(FMODEvents.instance.footSteps, transform.position);
        }
    }
}
