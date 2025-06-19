using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

[Serializable, VolumeComponentMenu("Post-processing/Custom/RadialDistortion")]
public class RadialDistortion : CustomPostProcessVolumeComponent, IPostProcessComponent
{
    [SerializeField]
    Material m_Material;

    [Tooltip("Controls the intensity of the effect.")]
    public ClampedFloatParameter intensity = new ClampedFloatParameter(0f, 0f, 1f);
    public NoInterpFloatParameter speed = new NoInterpFloatParameter(1f);
    public NoInterpFloatParameter clamping = new NoInterpFloatParameter(40f);
    public bool IsActive() => m_Material != null && intensity.value > 0f;
    public override CustomPostProcessInjectionPoint injectionPoint => CustomPostProcessInjectionPoint.AfterPostProcess;
    public override void Setup()
    {
        if (Shader.Find("Hidden/Shader/RadialDistortion") != null)
            m_Material = new Material(Shader.Find("Hidden/Shader/RadialDistortion"));
        else
            Debug.LogError("Shader not found: Hidden/Shader/RadialDistortion. Please ensure the shader is correctly placed in the project.", this);
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
        m_Material.SetFloat("_Speed", speed.value);
        m_Material.SetFloat("_Clamping", clamping.value);
        HDUtils.DrawFullScreen(cmd, m_Material, destination);
        //m_Material.DisableKeyword("_DRAW_FULLSCREEN");
        //cmd.Blit(source, destination, m_Material, 0);
    }
    public override void Cleanup()
    {
        CoreUtils.Destroy(m_Material);
    }

}
