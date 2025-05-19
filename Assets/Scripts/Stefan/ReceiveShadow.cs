using UnityEngine;
[ExecuteInEditMode]
public class ReceiveShadow : MonoBehaviour
{
    [SerializeField] bool _receiveShadows;
    bool _lastVal;
    void Start()
    {
        GetComponent<Renderer>().receiveShadows = _receiveShadows;
    }

    void Update()
    {
        if(_lastVal != _receiveShadows)
        {
            _lastVal = _receiveShadows;
            SetReceiveShadows();
        }
    }

    void SetReceiveShadows()
    {
        var renderer = GetComponent<Renderer>();
        renderer.receiveShadows = _receiveShadows;
        Debug.Log("setting shadow: " + renderer.receiveShadows);

    }
}
