using UnityEngine;

public class CrouchTrigger : MonoBehaviour
{
    [SerializeField] private GameObject crouchTrigger;

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "CrouchCube")
        {
            crouchTrigger.SetActive(true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.name == "CrouchCube")
        {
            crouchTrigger.SetActive(false);
        }
    }
}
