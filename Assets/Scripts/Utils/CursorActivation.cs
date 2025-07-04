using UnityEngine;

public class CursorActivation : MonoBehaviour
{
    [SerializeField] bool active;

    private void OnEnable()
    {
        if (active)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
