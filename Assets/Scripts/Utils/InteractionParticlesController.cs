using UnityEngine;

public class InteractionParticlesController : MonoBehaviour
{
    [SerializeField] ParticleSystem _pickUpParticles;
    public void OnItemPickup(GameObject obj, IPickupable pickupable)
    {
        _pickUpParticles.gameObject.SetActive(true);
        //Vector3 originalPosition = _pickUpParticles.transform.position;
        _pickUpParticles.transform.position = obj.transform.position;
        _pickUpParticles.Play();
        //_pickUpParticles.transform.position = originalPosition;
    }

}
