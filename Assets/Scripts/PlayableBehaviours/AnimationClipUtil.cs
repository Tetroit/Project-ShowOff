using System.Collections.Generic;
#if UNITY_EDITOR
    using UnityEditor.Timeline;
#endif
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using static UnityEngine.GraphicsBuffer;

[System.Serializable]
public class AnimationClipContext : MonoBehaviour
{
    [field: SerializeField]
    public PlayableDirector playableDirectorContext { get; private set; }
    public TimelineAsset timelineAssetContext;
    public AnimationTrack animationTrackContext;
    public TimelineClip timelineClipContext;
    public AnimationPlayableAsset animationPlayableAsset;
    public AnimationClip animationClipContext;

    public List<TimelineClip> removeAfterOneNextPlay;
    public void Bind(PlayableDirector director)
    {
        playableDirectorContext = director;
        director.stopped += DeleteAfter1Play;
    }
    public void Unbind()
    {
        if (playableDirectorContext == null)
            return;
        playableDirectorContext.stopped -= DeleteAfter1Play;
        playableDirectorContext = null;
    }
    public bool IsPlayableDirectorBound()
    {
        return playableDirectorContext != null;
    }

    private void OnDisable()
    {
        Unbind();
    }

    /// <summary>
    /// Gets an animation clip from <seealso cref="timelineAssetContext"/> by name.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public AnimationClip GetAnimationClip(string name)
    {
        IEnumerable<TimelineClip> clips;
        if (animationClipContext != null)
        {

            clips = animationTrackContext.GetClips();
            foreach (var clip in clips)
            {
                if (clip.animationClip.name == name)
                {
                    return GetAnimationClip(clip);
                }
            }
        }
        foreach (var track in timelineAssetContext.GetOutputTracks())
        {
            if (track is AnimationTrack)
            {
                AnimationTrack animTrack = track as AnimationTrack;
                //animationTrackContext = animTrack;

                clips = animTrack.GetClips();
                foreach (var clip in clips)
                {

                    if (clip.animationClip.name == name)
                    {
                        animationTrackContext = animTrack;
                        return GetAnimationClip(clip);
                    }
                }
            }
        }
        Debug.Log("No animation clip found");
        return null;
    }
    /// <summary>
    /// Gets an animation clip from <paramref name="track"/> by name.
    /// </summary>
    /// <param name="track"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public AnimationClip GetAnimationClip(AnimationTrack track, string name)
    {
        animationTrackContext = track;
        return GetAnimationClip(track.name);
    }
    public AnimationClip GetAnimationClip(TimelineClip clip)
    {
        animationClipContext = clip.animationClip;
        return clip.animationClip;
    }

    double lastPlayTime;

    /// <summary>
    /// Sets <paramref name="clip"/> to the <seealso cref="animationTrackContext"/> at <paramref name="time"/>.
    /// </summary>
    /// <param name="clip"></param>
    /// <param name="time"></param>
    public void AddClip(AnimationClip clip, float time = 0.0f, Vector3? position = null, Vector3? eulerAngles = null,
        float fadeInTime = 0, float fadeOutTime = 0, float duration = -1, bool startOffset = false, bool deleteAfterPlay = true)
    {

        if (playableDirectorContext != null)
        {
            lastPlayTime = playableDirectorContext.time;
        }

        bool shouldSetOffsets = position.HasValue || eulerAngles.HasValue;
        TimelineClip newClip = animationTrackContext.CreateClip(clip);
        newClip.start = time;
        newClip.displayName = clip.name;

        newClip.blendInDuration = fadeInTime;
        newClip.blendOutDuration = fadeOutTime;
        if (duration > 0)
            newClip.duration = duration;

        animationPlayableAsset = (AnimationPlayableAsset)newClip.asset;
        if (shouldSetOffsets)
        {
            if (position.HasValue)
                animationPlayableAsset.position = position.Value;
            if (eulerAngles.HasValue)
                animationPlayableAsset.rotation = Quaternion.Euler(eulerAngles.Value);
        }

        animationPlayableAsset.removeStartOffset = !startOffset;

        if (deleteAfterPlay)
        {
            removeAfterOneNextPlay.Add(newClip);
        }

        Debug.Log("clip inserted: " + clip.name + " at time: " + time + " on track: " + animationTrackContext.name);
    }

    public void RefreshDirector(TimelineAsset timelineAsset = null)
    {
        bool shouldResume = playableDirectorContext.state == PlayState.Playing;
        if (timelineAsset == null)
            timelineAsset = (TimelineAsset)playableDirectorContext.playableAsset;
        if (shouldResume)
        {
            lastPlayTime = playableDirectorContext.time;
            playableDirectorContext.Stop();
        }

        playableDirectorContext.time = 0;



        Bind(playableDirectorContext);
#if UNITY_EDITOR
        TimelineEditor.Refresh(RefreshReason.ContentsAddedOrRemoved);
#endif

        playableDirectorContext.playableAsset = null;

        playableDirectorContext.RebuildGraph();

        playableDirectorContext.playableAsset = timelineAsset;

        if (shouldResume)
            playableDirectorContext.time = lastPlayTime;

        foreach (var binding in timelineAsset.outputs)
        {
            var track = binding.sourceObject;
            var existingBinding = playableDirectorContext.GetGenericBinding(track);

            if (existingBinding != null)
            {
                // Re-apply it to ensure Unity links it properly in the graph
                playableDirectorContext.SetGenericBinding(track, existingBinding);
            }
        }

        playableDirectorContext.RebindPlayableGraphOutputs();
        playableDirectorContext.Evaluate();
        if (shouldResume)
            playableDirectorContext.Play();
    }
    /// <summary>
    /// Requires <seealso cref="animationTrackContext"/> and <seealso cref="timelineClipContext"/> to be set."/>
    /// </summary>
    public void DeleteClip()
    {
        if (timelineAssetContext == null || timelineClipContext == null)
            return;
        timelineAssetContext.DeleteClip(timelineClipContext);
        timelineClipContext = null;
    }
    public void DeleteClip(TimelineClip clipToRemove)
    {
        timelineClipContext = clipToRemove;
        DeleteClip();
    }

    public void DeleteAfter1Play(PlayableDirector director)
    {
        for (int i = 0; i < removeAfterOneNextPlay.Count;)
        {
            TimelineClip clipToRemove = removeAfterOneNextPlay[i];
            if (clipToRemove == null)
                continue;
            DeleteClip(clipToRemove);
            removeAfterOneNextPlay.Remove(clipToRemove);
        }
        RefreshDirector();
    }
    /// <summary>
    /// Requires <seealso cref="timelineAssetContext"/> to be set.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public AnimationTrack GetAnimationTrack(string name)
    {
        foreach (var track in timelineAssetContext.GetOutputTracks())
        {
            if (track is AnimationTrack)
            {
                AnimationTrack animTrack = track as AnimationTrack;
                if (animTrack.name == name)
                {
                    animationTrackContext = animTrack;
                    return animationTrackContext;
                }
            }
        }
        return null;
    }





#if UNITY_EDITOR
    public void GetAnimationPlayableAsset()
    {
        animationPlayableAsset = (AnimationPlayableAsset)timelineClipContext.asset;
    }
    public AnimationTrackInsertionInfo GetAnimationTrackInsertInfo()
    {
        return GetAnimationTrackInsertInfo(inputField);
    }
    public AnimationTrackInsertionInfo GetAnimationTrackInsertInfo(string name)
    {
        GetTimelineClipFromAnimationTrack(name);
        GetAnimationPlayableAsset();
        GetAnimationClip(timelineClipContext);
        AnimationTrackInsertionInfo atii = new AnimationTrackInsertionInfo
        {
            trackName = animationTrackContext.name,
            clip = animationClipContext,
            insertTime = (float)timelineClipContext.start,
            positionOffset = animationPlayableAsset.position,
            rotationOffset = animationPlayableAsset.rotation.eulerAngles,
            fadeInTime = (float)timelineClipContext.blendInDuration,
            fadeOutTime = (float)timelineClipContext.blendOutDuration,
            duration = (float)timelineClipContext.duration,
            startOffset = !animationPlayableAsset.removeStartOffset
        };
        generatedATII = atii;
        return atii;
    }
    public void GetTimelineClipFromAnimationTrack()
    {
        GetTimelineClipFromAnimationTrack(inputField);
    }
    public void GetTimelineClipFromAnimationTrack(string name)
    {
        var clips = animationTrackContext.GetClips();
        foreach (var clip in clips)
        {
            if (clip.animationClip.name == inputField)
            {
                timelineClipContext = clip;
                return;
            }
        }
    }

    /// <summary>
    /// Path is relative to Assets folder.
    /// </summary>
    /// <param name="path"></param>
    public AnimationClip LoadClip(string path)
    {
        animationClipContext = UnityEditor.AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
        return animationClipContext;
    }
    public TimelineAsset LoadTimeline(string path)
    {
        timelineAssetContext = UnityEditor.AssetDatabase.LoadAssetAtPath<TimelineAsset>(path);
        return timelineAssetContext;
    }

    [Space(20)]
    [Header("Editing")]
    public string inputField;
    public List<string> list;
    public AnimationTrackInsertionInfo generatedATII;

    [ContextMenu("Get Animation Tracks")]
    public void GetAnimationTracks()
    {
        list = new List<string>();
        foreach (var track in timelineAssetContext.GetOutputTracks())
        {
            if (track is AnimationTrack)
            {
                AnimationTrack animTrack = track as AnimationTrack;
                list.Add(animTrack.name);
            }
        }
    }

    [ContextMenu("Get Animation Clips")]
    public void GetAnimationClips()
    {
        list = new List<string>();
        foreach (var track in animationTrackContext.GetClips())
        {
            list.Add(track.animationClip.name); 
        }
    }

    [ContextMenu("Get Timeline Path")]
    public void GetTimelinePath()
    {
        if (timelineAssetContext == null)
            return;
        list.Clear();
        list.Add(UnityEditor.AssetDatabase.GetAssetPath(timelineAssetContext));
    }
#endif
}