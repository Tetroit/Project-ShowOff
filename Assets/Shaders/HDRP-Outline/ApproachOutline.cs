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
        if ((cam.transform.position - transform.position).magnitude < radius && !inArea)
        {
            inArea = true;
            OutlinePass.OutlineRenderers.Add(GetComponent<Renderer>());
        }

        if ((cam.transform.position - transform.position).magnitude > radius && inArea)
        {
            inArea = false;
            OutlinePass.OutlineRenderers.Remove(GetComponent<Renderer>());
        }
    }
    private void OnEnable()
    {
        cam = Camera.main;
    }

    private void OnDisable()
    {
        OutlinePass.OutlineRenderers?.Remove(GetComponent<Renderer>());
    }
}
