using UnityEngine;

public class Note : MonoBehaviour, IHoldable
{
    [SerializeField] string _text;

    public Transform Self => transform;

    public void Deselect()
    {

    }

    public void Hold()
    {
        Debug.Log("showing text");
    }
}
