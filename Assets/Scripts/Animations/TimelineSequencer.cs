using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Timeline;
using UnityEngine.Playables;
using UnityEngine.Events;

public class TimelineSequencer : MonoBehaviour
{
    [SerializeField]
    List<TimelineAsset> timelines = new();
    int current;

    [SerializeField]
    PlayableDirector director;

    List<TimelineAsset> deleteAfterPlaying = new();

    public UnityEvent finished;
    public bool isPlaying { get; private set; }

    public void RunOn(PlayableDirector director)
    {
        this.director = director;
        Run();
    }

    public void Run()
    {
        if (timelines.Count == 0)
        {
            Debug.Log("No timelines queued.", this);
            return;
        }
        current = 0;
        director.playableAsset = timelines[current];
        director.stopped += NextInSequence;
        director.Play();

        Debug.Log("Sequence started.", this);
    }

    void NextInSequence(PlayableDirector director)
    {
        current++;

        //finished
        if (current >= timelines.Count)
        {
            finished?.Invoke();
            OnStopPlaying();

            Debug.Log("Sequence finished execution.", this);
        }
        //continue execution
        else
        {
            director.playableAsset = timelines[current];
            if (director.state != PlayState.Playing)
            {
                director.Play();
            }

            Debug.Log($"Playing next in queue. ({current})", this);
        }
    }

    [ContextMenu("Terminate")]
    public void Terminate()
    {
        if (isPlaying)
        {
            director.Stop();
            OnStopPlaying();
            Debug.Log("Sequence terminated.", this);
        }
    }

    void OnStopPlaying()
    {
        director.stopped -= NextInSequence;
        isPlaying = false;
        for (int i = 0; i < deleteAfterPlaying.Count;)
        {
            var it = deleteAfterPlaying[i];
            timelines?.Remove(it);
            deleteAfterPlaying.RemoveAt(i);
        }
    }
    public void StageTimelineAfterCurrent(TimelineAsset timelineAsset)
    {
        timelines.Add(timelineAsset);
    }
    public void StageRadioactiveTimelineAfterCurrent(TimelineAsset timelineAsset)
    {
        timelines.Add(timelineAsset);
        deleteAfterPlaying.Add(timelineAsset);
    }

    private void OnDisable()
    {
        Terminate();
    }
}
