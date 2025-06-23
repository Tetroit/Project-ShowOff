using UnityEngine;

public class FootstepsPlayer : MonoBehaviour
{
    private LayerMask mask;
    private int terrain;
    //0 = ground, 1 = stone, 2 = wood

    private void Start()
    {
        mask = LayerMask.GetMask("Ground", "Wood", "Stone");
    }

    public void PlayFootstepSound()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 1000.0f, mask))
        {
            if(hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                terrain = 0;
            }
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Wood"))
            {
                terrain = 1;
            }
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Stone"))
            {
                terrain = 2;
            }
        }
        FMOD.Studio.EventInstance e = FMODUnity.RuntimeManager.CreateInstance(FMODEvents.instance.footSteps);
        e.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(transform.position));
        e.setParameterByName("Terrain", terrain, true);
        e.start();
        e.release();
    }

    public void PlayLadderSound()
    {
        AudioManager.instance.PlayOneShot(FMODEvents.instance.ladder, transform.position);
    }
}
