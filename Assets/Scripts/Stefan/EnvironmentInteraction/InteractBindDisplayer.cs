using TMPro;
using UnityEngine;

public class InteractBindDisplayer : MonoBehaviour
{
    [SerializeField] GameObject _holdView;
    [SerializeField] GameObject _openView;

    public void OnHoverEnter(GameObject obj, IInteractable component)
    {
        _holdView.SetActive(true);
        _holdView.GetComponentInChildren<TextMeshPro>().text = "E to Interact";
        _holdView.transform.position = obj.transform.position + Vector3.up * .4f;
    }

    public void OnHover(GameObject obj, IInteractable component)
    {
        _holdView.transform.LookAt(transform);
    }

    public void OnHoverExit(GameObject obj, IInteractable component)
    {
        _holdView.SetActive(false);

    }
}
