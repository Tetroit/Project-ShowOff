using UnityEngine;

public class EnvironmentDisbaler : MonoBehaviour
{
    [SerializeField] GameObject _outside;

    void OnTriggerExit(Collider player)
    {
        bool goingUp = Vector3.Dot(Vector3.up, player.transform.position - transform.position) > 0;
        StartCoroutine(DeferredSetOutside(goingUp));
    }

    System.Collections.IEnumerator DeferredSetOutside(bool active)
    {
        yield return null; // Wait 1 frame
        _outside.SetActive(active);
    }
}
