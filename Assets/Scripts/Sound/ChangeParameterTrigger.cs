using UnityEngine;

public class ChangeParameterTrigger : MonoBehaviour
{
    [SerializeField] private float newValue = 1f;
    [SerializeField] private string parameterName;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            AudioManager.instance.SetAmbienceByParameter(parameterName, newValue);
            Debug.Log("changed" + parameterName);

        }
    }
}
