using Unity.VisualScripting;
using UnityEngine;

public class floorbreakdraft : MonoBehaviour
{
    float i = 0;
    private void OnCollisionEnter(Collision collision)
    {
        if (i >= 1)
            Destroy(gameObject);
       
    }
    
}
