using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AnimationClipContext))]
public class AnimationBranch : MonoBehaviour
{
    [System.Serializable]
    public struct AnimationTrackInsertionInfo
    {
        public float insertTime;
        public string trackName;
        public AnimationClip clip;
    }

    [Header("Success Clips")]
    public List<AnimationTrackInsertionInfo> successAnimationClips;

    [Header("Fail Clips")]
    public List<AnimationTrackInsertionInfo> failAnimationClips;
    
    [SerializeField] float branchTime;
    public AnimationClipContext animationClipContext;

    [SerializeField]
    List<string> animationTracks;
    public void Decide(bool success)
    {
        if (animationClipContext == null)
            animationClipContext.GetAnimationTrack("rat");
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
            animationClipContext.AddClip(track.clip, track.insertTime);
        }
    }

    [ContextMenu("Set Fail Clip")]
    void SetFail()
    {
        foreach (var track in failAnimationClips)
        {
            animationClipContext.GetAnimationTrack(track.trackName);
            animationClipContext.AddClip(track.clip, track.insertTime);
        }
    }
}
