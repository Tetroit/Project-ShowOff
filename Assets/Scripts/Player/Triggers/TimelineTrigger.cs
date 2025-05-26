using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class TimelineTrigger<TargetType> : SimpleTrigger<TargetType>
    where TargetType : Component
{
    public TimelineAsset asset;
    public PlayableDirector director;
    public UnityAction OnAnimationEnd;
    public override void Trigger()
    {
        director.playableAsset = asset;
        director.Play();
        director.stopped += OnEndInternal;
    }

    protected override bool NullHandling(TargetType target)
    {
        if (target == null)
        {
            Debug.LogError("Target object was null", this);
            return false;
        }
        if (director == null)
        {
            Debug.LogError("Target director was null", this);
            return false;
        }
        if (asset == null)
        {
            Debug.LogError("Asset was null", this);
            return false;
        }
        return true;
    }
    void OnEndInternal(PlayableDirector finishedDirector)
    {
        OnAnimationEnd?.Invoke();
        AnimationEnd();
        Debug.Log("Timeline animation ended", this);
    }
    protected virtual void AnimationEnd()
    {
    }
}
