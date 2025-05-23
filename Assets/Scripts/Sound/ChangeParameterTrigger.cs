using UnityEngine;

public class ChangeParameterTrigger : MonoBehaviour
{
    [SerializeField] float newValue = 1f;
    [SerializeField] string parameterName;
    [SerializeField] bool _debug;

    void OnTriggerEnter(Collider other)
    {
        AudioManager.instance.SetAmbienceByParameter(parameterName, newValue);
        
        if(_debug )
            Debug.Log("changed" + parameterName);

    }
}
