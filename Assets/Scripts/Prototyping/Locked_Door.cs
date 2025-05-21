using UnityEngine;
using UnityEngine.ProBuilder.Shapes;

public class Locked_Door : MonoBehaviour
{
    public bool Locked = true;
    public GameObject door;
    private Collider doorCollider;

    void Start()
    {
        doorCollider = GetComponent<Collider>();
    }
   
    void Update()
    {
        if (Locked)
        { 
            door.SetActive(true);
            doorCollider.enabled = true;
        }
        else
        {    
            door.SetActive(false);
            doorCollider.enabled = false;
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            UnlockDoor();
        }
        else if (Input.GetKeyDown(KeyCode.L))
        {
            CloseDoor();
        }
    }
    public void UnlockDoor()
    {
        Locked = false;
    }
    public void CloseDoor()
    {
        Locked = true;
    }
}

