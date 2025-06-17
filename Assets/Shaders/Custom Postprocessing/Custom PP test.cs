using UnityEngine;
using UnityEngine.Rendering;

public class CustomPPtest : VolumeComponent
{
    public ClampedFloatParameter intensity = new ClampedFloatParameter(0f, 0f, 1f);
    public BoolParameter enableEffect = new BoolParameter(false);
}
