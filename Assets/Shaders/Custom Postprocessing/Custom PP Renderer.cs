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
    public Texture2DParameter texture = new Texture2DParameter(null);
    public Vector2Parameter scale = new Vector2Parameter(Vector2.one);
    public Vector2Parameter offset = new Vector2Parameter(Vector2.zero);

    public FloatParameter twist = new FloatParameter(0.01f);
    public FloatParameter twistOffset = new FloatParameter(0f);
    public FloatParameter twistScale = new FloatParameter(10f);
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
        m_Material.SetFloat("_Twist", twist.value);
        m_Material.SetFloat("_TwistOffset", twistOffset.value);
        m_Material.SetFloat("_TwistScale", twistScale.value);
        m_Material.SetTexture("_MainTex", source);
        if (texture.value == null)
        {
            m_Material.DisableKeyword("_OVERLAY_ON");
        }
        else
        {
            m_Material.EnableKeyword("_OVERLAY_ON");
            m_Material.SetTexture("_Overlay", texture.value);
            m_Material.SetVector("_Scale", scale.value);
            m_Material.SetVector("_Offset", offset.value);
        }
        HDUtils.DrawFullScreen(cmd, m_Material, destination);
    }
    public override void Cleanup()
    {
        CoreUtils.Destroy(m_Material);
    }

}
