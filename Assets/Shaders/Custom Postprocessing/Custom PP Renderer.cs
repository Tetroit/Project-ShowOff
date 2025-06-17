using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using static Unity.VisualScripting.Member;

[Serializable, VolumeComponentMenu("Post-processing/Custom/GrayScale")]
public class CustomPPRenderer : CustomPostProcessVolumeComponent, IPostProcessComponent
{
    [SerializeField]
    Material m_Material;

    [Tooltip("Controls the intensity of the effect.")]
    public ClampedFloatParameter intensity = new ClampedFloatParameter(0f, 0f, 1f);
    public bool IsActive() => m_Material != null && intensity.value > 0f;
    public override CustomPostProcessInjectionPoint injectionPoint => CustomPostProcessInjectionPoint.AfterPostProcess;
    public override void Setup()
    {
        if (Shader.Find("Hidden/Shader/CustomPP") != null)
            m_Material = new Material(Shader.Find("Hidden/Shader/CustomPP"));
        else
            Debug.LogError("Shader not found: Hidden/Shader/GrayScale. Please ensure the shader is correctly placed in the project.", this);
    }
    public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle source, RTHandle destination)
    {
        if (m_Material == null)
            return;
        m_Material.SetFloat("_Intensity", intensity.value);
        m_Material.SetTexture("_MainTex", source);
        HDUtils.DrawFullScreen(cmd, m_Material, destination);
    }
    public override void Cleanup()
    {
        CoreUtils.Destroy(m_Material);
    }

}
