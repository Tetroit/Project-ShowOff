using System;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

[Serializable, VolumeComponentMenu("Post-processing/Custom/Blink")]
public class Blink : CustomPostProcessVolumeComponent, IPostProcessComponent
{
    [SerializeField]
    Material m_Material;
    RTHandle m_TempWithMips; 
    
    private RTHandle halfResTarget;

    [Tooltip("Controls the intensity of the effect.")]
    public ClampedFloatParameter intensity = new ClampedFloatParameter(0f, 0f, 1f);
    public NoInterpFloatParameter closedFac = new NoInterpFloatParameter(1f); 
    public ClampedFloatParameter blurRadius = new ClampedFloatParameter(0f, 0f, 10f);

    public IntParameter sampleCount = new IntParameter(9); // Number of samples for blur
    public BoolParameter downSample = new BoolParameter(true); // Downsample for performance
    public bool IsActive() => m_Material != null && intensity.value > 0f;
    public override CustomPostProcessInjectionPoint injectionPoint => CustomPostProcessInjectionPoint.AfterPostProcessBlurs;
    public override void Setup()
    {
        if (Shader.Find("Hidden/Shader/Blink") != null)
            m_Material = new Material(Shader.Find("Hidden/Shader/Blink"));
        else
            Debug.LogError("Shader not found: Hidden/Shader/Blink. Please ensure the shader is correctly placed in the project.", this);
    }
    public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle source, RTHandle destination)
    {
        if (m_Material == null)
        {
            Debug.LogError("Material is null. Please ensure the shader is correctly set up.", this);
            return;
        }
        m_Material.SetTexture("_MainTex", source);
        m_Material.SetFloat("_Intensity", intensity.value);

        m_Material.EnableKeyword("_DRAW_FULLSCREEN");
        HDUtils.DrawFullScreen(cmd, m_Material, destination);
    }
    public override void Cleanup()
    {
        CoreUtils.Destroy(m_Material);
    }

}
