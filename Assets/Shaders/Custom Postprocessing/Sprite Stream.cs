using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using static Unity.VisualScripting.Member;

[Serializable, VolumeComponentMenu("Post-processing/Custom/SpriteStream")]
public class SpriteStream : CustomPostProcessVolumeComponent, IPostProcessComponent
{
    [SerializeField]
    Material m_Material;

    [Tooltip("Controls the intensity of the effect.")]
    public ClampedFloatParameter intensity = new ClampedFloatParameter(0f, 0f, 1f);
    public NoInterpFloatParameter speed = new NoInterpFloatParameter(1f);
    public Texture2DParameter texture = new Texture2DParameter(null);
    public NoInterpColorParameter colorMask = new NoInterpColorParameter(Color.white, false, false, true);
    public NoInterpVector2Parameter scale = new NoInterpVector2Parameter(Vector2.one, false);
    public NoInterpVector2Parameter offset = new NoInterpVector2Parameter(Vector2.zero);

    public NoInterpFloatParameter twist = new NoInterpFloatParameter(0.01f);
    public NoInterpFloatParameter twistOffset = new NoInterpFloatParameter(0f);
    public NoInterpFloatParameter twistScale = new NoInterpFloatParameter(10f);
    public NoInterpVector2Parameter maskMinMax = new NoInterpVector2Parameter(new Vector2(0.2f, 1.4f));
    public bool IsActive() => m_Material != null && intensity.value > 0f;
    public override CustomPostProcessInjectionPoint injectionPoint => CustomPostProcessInjectionPoint.AfterPostProcess;

    public override void Setup()
    {
        if (Shader.Find("Hidden/Shader/SpriteStream") != null)
            m_Material = new Material(Shader.Find("Hidden/Shader/SpriteStream"));
        else
            Debug.LogError("Shader not found: Hidden/Shader/SpriteStream. Please ensure the shader is correctly placed in the project.", this);

        scale.overrideState = false;
    }
    public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle source, RTHandle destination)
    {
        if (m_Material == null)
            return;
        m_Material.SetFloat("_Intensity", intensity.value);
        m_Material.SetFloat("_Speed", speed.value);
        m_Material.SetFloat("_Twist", twist.value);
        m_Material.SetFloat("_TwistOffset", twistOffset.value);
        m_Material.SetFloat("_TwistScale", twistScale.value);

        m_Material.SetFloat("_MaskMin", maskMinMax.value.x);
        m_Material.SetFloat("_MaskMax", maskMinMax.value.y);
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
            m_Material.SetColor("_ColorMask", colorMask.value);
        }
        m_Material.EnableKeyword("_DRAW_FULLSCREEN");
        HDUtils.DrawFullScreen(cmd, m_Material, destination);
        //m_Material.DisableKeyword("_DRAW_FULLSCREEN");
        //cmd.Blit(source, destination, m_Material, 0);
    }
    public override void Cleanup()
    {
        CoreUtils.Destroy(m_Material);
    }

}
