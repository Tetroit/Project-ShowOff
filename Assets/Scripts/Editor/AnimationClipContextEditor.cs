using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AnimationClipContext))]
public class AnimationClipContextEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();


        AnimationClipContext context = (AnimationClipContext)target;

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Load Timeline Asset"))
        {
            string path = context.inputField;

            if (AssetDatabase.AssetPathExists(path))
            {
                context.LoadTimeline(path);
            }
            else
            {
                Debug.LogWarning($"Invalid path: {path}, make sure your path is relative to assets", this);
            }
        }

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Get Animation Track"))
        {
            if (context.timelineAssetContext == null)
            {
                Debug.LogWarning("Timeline Asset Context is not set. Please set it before getting tracks.", this);
                return;
            }
            var clip = context.GetAnimationTrack(context.inputField);
            if (clip == null)
            {
                Debug.LogWarning($"No animation track found with name: {context.inputField}", this);
            }
        }


        if (GUILayout.Button("Get All Animation Track Names"))
        {
            if (context.timelineAssetContext == null)
            {
                Debug.LogWarning("Timeline Asset Context is not set. Please set it before getting tracks.", this);
                return;
            }
            context.GetAnimationTracks();
        }

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Load Animation Clip"))
        {
            string path = context.inputField;
            
            if (AssetDatabase.AssetPathExists(path))
            {
                context.LoadClip(path);
            }
            else
            {
                Debug.LogWarning($"Invalid path: {path}, make sure your path is relative to assets", this);
            }
        }

        if (GUILayout.Button("Get Animation Clip"))
        {
            if (context.animationTrackContext == null)
            {
                Debug.LogWarning("Animation Track Context is not set. Please set it before getting clips.", this);
                return;
            }
            var clip = context.GetAnimationClip(context.inputField);
            if (clip == null)
            {
                Debug.LogWarning($"No animation clip found with name: {context.inputField}", this);
            }
        }

        if (GUILayout.Button("Get All Animation Clip Names"))
        {
            if (context.animationTrackContext == null)
            {
                Debug.LogWarning("Animation Track Context is not set. Please set it before getting clips.", this);
                return;
            }
            context.GetAnimationClips();
        }

        GUILayout.EndHorizontal();

    }
}
