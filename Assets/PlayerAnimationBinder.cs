using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class PlayerAnimationBinder : MonoBehaviour
{
    [System.Serializable]
    public struct PlayableDirectorToString
    {
        public string key;
        public PlayableDirector director;
    }

    [System.Serializable]
    public struct TimelineToString
    {
        public string key;
        public TimelineAsset asset;
    }

    [SerializeField] List<PlayableDirectorToString> directors;
    [SerializeField] List<TimelineToString> timelines;

    public PlayableDirector GetDirector(string key)
    {
        var item = directors?.Find(d => d.key == key);
        if (item != null)
            return item?.director;

        Debug.LogWarning($"PlayableDirector with key '{key}' not found.", this);
        return null;
    }

    public TimelineAsset GetTimeline(string key)
    {
        var item = timelines?.Find(t => t.key == key);
        if (item != null)
            return item?.asset;

        Debug.LogWarning($"TimelineAsset with key '{key}' not found.", this);
        return null;
    }
}
