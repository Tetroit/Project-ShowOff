using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct AnimationTrackInsertionInfo
{
    public float insertTime;
    public string trackName;
    public AnimationClip clip;
    public Vector3 positionOffset;
    public Vector3 rotationOffset;
    public float fadeInTime;
    public float fadeOutTime;
    public float duration;
    public bool startOffset;
}

[RequireComponent(typeof(AnimationClipContext))]
public class AnimationBranch : MonoBehaviour
{

    [Header("Success Clips")]
    public List<AnimationTrackInsertionInfo> successAnimationClips;

    [Header("Fail Clips")]
    public List<AnimationTrackInsertionInfo> failAnimationClips;
    
    public AnimationClipContext animationClipContext;

    [SerializeField]
    List<string> animationTracks;
    public void Decide(bool success)
    {
        if (animationClipContext == null) return;
        if (success)
        {
            SetSuccess();
        }
        else
        {
            SetFail();
        }
    }

    [ExecuteInEditMode]
    private void OnValidate()
    {
        if (animationClipContext == null)
        {
            animationClipContext = GetComponent<AnimationClipContext>();
        }
    }

    [ContextMenu("Set Success Clip")]
    void SetSuccess()
    {
        foreach (var track in successAnimationClips)
        {
            animationClipContext.GetAnimationTrack(track.trackName);
            animationClipContext.AddClip(track.clip, track.insertTime, track.positionOffset, track.rotationOffset, track.fadeInTime, track.fadeOutTime, track.duration <= 0 ? -1 : track.duration, track.startOffset);
        }
    }

    [ContextMenu("Set Fail Clip")]
    void SetFail()
    {
        foreach (var track in failAnimationClips)
        {
            animationClipContext.GetAnimationTrack(track.trackName);
            animationClipContext.AddClip(track.clip, track.insertTime, track.positionOffset, track.rotationOffset, track.fadeInTime, track.fadeOutTime, track.duration <= 0 ? -1 : track.duration, track.startOffset);
        }
    }
}
