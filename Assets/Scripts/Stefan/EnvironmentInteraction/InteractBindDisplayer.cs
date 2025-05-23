using TMPro;
using UnityEngine;

public class InteractBindDisplayer : MonoBehaviour
{
    [SerializeField] GameObject _holdViewPrefab;
    TextMeshPro _holdView;

    private void Awake()
    {
        _holdView = Instantiate(_holdViewPrefab).GetComponentInChildren<TextMeshPro>(true);
    }

    public void OnHoverEnter(GameObject obj, IInteractable component)
    {
        _holdView.gameObject.SetActive(true);
        _holdView.text = "E to Interact";
        _holdView.transform.position = obj.transform.position + Vector3.up * .4f;
    }

    public void OnHover(GameObject obj, IInteractable component)
    {
        _holdView.transform.LookAt(transform);
    }

    public void OnHoverExit(GameObject obj, IInteractable component)
    {
        _holdView.gameObject.SetActive(false);

    }
}
