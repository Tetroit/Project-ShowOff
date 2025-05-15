using FMODUnity;
using UnityEngine;

public class MoveAmbience : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("moved?");
            AudioManager.instance.ambienceEventInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
        }
    }
}
