using Modules.Rendering.Outline;
using UnityEngine;


[ExecuteInEditMode]
[RequireComponent(typeof(Renderer))]
public class OutlineComponent : MonoBehaviour
{
    [SerializeField]
    float radius;
    bool inArea;
    Camera cam;
    public void Update()
    {
        if ((cam.transform.position - transform.position).magnitude < radius && !inArea) Show();

        if ((cam.transform.position - transform.position).magnitude > radius && inArea) Hide();
    }
    void Show()
    {
        inArea = true;
        OutlinePass.OutlineRenderers.Add(GetComponent<Renderer>());
    }
    void Hide()
    {
        OutlinePass.OutlineRenderers.Remove(GetComponent<Renderer>());
        inArea = false;
    }
    private void OnEnable()
    {
        cam = Camera.main;

        if ((cam.transform.position - transform.position).magnitude < radius) Show();
        if ((cam.transform.position - transform.position).magnitude > radius) Hide();
    }

    private void OnDisable()
    {
        OutlinePass.OutlineRenderers?.Remove(GetComponent<Renderer>());
    }
}
