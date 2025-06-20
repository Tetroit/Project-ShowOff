using Mono.Cecil;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[System.Serializable]
public class AnimationClipContext : MonoBehaviour
{
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
        playableDirectorContext.stopped += DeleteAfter1Play;
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
    /// Path is relative to Assets folder.
    /// </summary>
    /// <param name="path"></param>
    public AnimationClip LoadClip(string path)
    {
        animationClipContext = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
        return animationClipContext;
    }
    public TimelineAsset LoadTimeline(string path)
    {
        timelineAssetContext = AssetDatabase.LoadAssetAtPath<TimelineAsset>(path);
        return timelineAssetContext;
    }
    /// <summary>
    /// Gets an animation clip from <seealso cref="timelineAssetContext"/> by name.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public AnimationClip GetAnimationClip(string name)
    {
        var clips = animationTrackContext.GetClips();
        foreach (var clip in clips)
        {
            if (clip.animationClip.name == name)
            {
                GetAnimationClip(clip);
            }
        }
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
    /// <summary>
    /// Sets <paramref name="clip"/> to the <seealso cref="animationTrackContext"/> at <paramref name="time"/>.
    /// </summary>
    /// <param name="clip"></param>
    /// <param name="time"></param>
    public void AddClip(AnimationClip clip, float time = 0.0f, Vector3? position = null, Vector3? eulerAngles = null,
        float fadeInTime = 0, float fadeOutTime = 0, float duration = -1, bool startOffset = false, bool deleteAfterPlay = true)
    {
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
        Bind(GetComponent<PlayableDirector>());
        double directorTime = playableDirectorContext.time;
        playableDirectorContext.RebuildGraph();
        playableDirectorContext.time = directorTime;
        playableDirectorContext.Evaluate();

        if (deleteAfterPlay)
        {
            removeAfterOneNextPlay.Add(newClip);
        }

        Debug.Log("clip inserted: " + clip.name + " at time: " + time + " on track: " + animationTrackContext.name);
    }
    /// <summary>
    /// Requires <seealso cref="animationTrackContext"/> and <seealso cref="timelineClipContext"/> to be set."/>
    /// </summary>
    public void DeleteClip()
    {
        if (timelineAssetContext == null || timelineClipContext == null)
            return;
        timelineAssetContext.DeleteClip(timelineClipContext);

    }
    public void DeleteClip(TimelineClip clipToRemove)
    {
        timelineClipContext = clipToRemove;
        DeleteClip();
    }

    public void DeleteAfter1Play(PlayableDirector director)
    {
        for (int i = 0; i < removeAfterOneNextPlay.Count; i++)
        {
            TimelineClip clipToRemove = removeAfterOneNextPlay[i];
            if (clipToRemove == null)
                continue;
            DeleteClip(clipToRemove);
            removeAfterOneNextPlay.Remove(clipToRemove);
        }
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

#if UNITY_EDITOR

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
        list.Add(AssetDatabase.GetAssetPath(timelineAssetContext));
    }
#endif
}