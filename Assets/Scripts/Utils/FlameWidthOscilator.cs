using UnityEngine;

public class FlameWidthOscillator : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public float baseWidth = 0.1f;
    public float oscillationAmplitude = 0.05f;
    public float oscillationSpeed = 3f;
    AnimationCurve _startCurve;
    AnimationCurve curve = new();

    private void Start()
    {
        _startCurve = lineRenderer.widthCurve;   
    }
    void Update()
    {
        float time = Time.time;
        float lineLength = lineRenderer.widthCurve.length;
        for (int i = 0; i < lineLength; i++)
        {
            float t = i / lineLength;
            float noise = Mathf.PerlinNoise((time + (1- t) * 3) * oscillationSpeed, 0f);
            float widthMultiplier = 1+(noise * oscillationAmplitude);

            curve.AddKey(new Keyframe(t, _startCurve[i].value * widthMultiplier));
        }

        lineRenderer.widthCurve = curve;
        curve.ClearKeys();
    }
}
