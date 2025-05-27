using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class PlayerAnimationBinder : MonoBehaviour
{
    [field: SerializeField] public PlayableDirector armDirector { get; private set; }
    [field: SerializeField] public PlayableDirector player { get; private set; }
    [field: SerializeField] public TimelineAsset armDoorOpeningTimeline { get; private set; }
    [field: SerializeField] public TimelineAsset fallingFloorAnimation { get; private set; }

}
