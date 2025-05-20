using UnityEngine;

public class SwapAmbience : MonoBehaviour
{
    [SerializeField] private AmbienceArea area;

    private void OnTriggerEnter(Collider other)
    {
        AudioManager.instance.SetAmbienceArea(area);
    }
}
