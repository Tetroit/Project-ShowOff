using System;
using UnityEngine;
using UnityEngine.Playables;


namespace amogus
{
    [Serializable]
    public class GhostChaseActivator : MonoBehaviour
    {
        public MillerController miller;
        [SerializeField] GameStateManager gameStateManager;
        [SerializeField] PlayerFSM fsm;
        [SerializeField] PlayableDirector director;

        public void Trigger()
        {
            gameStateManager.SwitchState<S_Cutscene>();
            miller.gameObject.SetActive(true);

            if (director != null)
            {
                director.stopped += DirectorAnimationEnd;
                director.Play();
            }
            else
                Invoke(nameof(AnimationEnd), 1f);

            var door = GetComponent<DoorCutsceneTrigger>();
            door.Lock();
            //play miller jump scare
            //play camera turn animation
        }

        public void DirectorAnimationEnd(PlayableDirector director)
        {
            director.stopped -= DirectorAnimationEnd;
            AnimationEnd();
        }
        public void AnimationEnd()
        {
            gameStateManager.SwitchToPrevious();
            miller.StartFollowing(fsm.transform);
            enabled = false;
        }
    }
}
