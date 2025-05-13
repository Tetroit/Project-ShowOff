using DG.Tweening;
using System.Collections;
using UnityEngine;

public class Note : MonoBehaviour, IHoldable
{
    [SerializeField] string _text;
    [SerializeField] float _returnTime;

    Vector3 _initialPosition;

    public Transform Self => transform;

    void Awake()
    {
        _initialPosition = transform.position;
    }

    public IEnumerator Deselect()
    {
        yield return null;
    }

    public IEnumerator Interact()
    {
        yield return null;
    }

    public Vector3 GetInitialPosition()
    {
        return _initialPosition;
    }
}
