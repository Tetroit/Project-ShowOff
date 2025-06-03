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
            gameStateManager.SwitchState(GameState.Cutscene);
            miller.gameObject.SetActive(true);

            if (director != null)
            {
                director.stopped += DirectorAnimationEnd;
                director.Play();
            }

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
        }
    }
}
