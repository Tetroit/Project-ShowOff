
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

namespace amogus
{
    [SelectionBase]
    public class DoorCutsceneTrigger : DoorTrigger
    {

        [SerializeField] protected bool noArm = false;

        public bool isOpen => door.isOpen;

        public void OpenInstant() => door.Open();
        public void CloseInstant() => door.Close();

        [Header("Animations")]
        [SerializeField] protected TimelineAsset closingCutscene;
        [SerializeField] protected TimelineAsset openingCutscene => closingCutscene;

        [SerializeField] protected DoorCutsceneAnimation cutscene;

        List<Action> afterAnimation = new();
        public void SetNoArm(bool val) => noArm = val;

        public override Predicate<PlayerFSM> Predicate => (PlayerFSM player) => {
            if (isLocked || cutscene.isRunningOn(door)) return false;
            return true;
        };

        bool FindBinder()
        {
            if (binder != null) return true;

            if (binder == null)
                binder = FindAnyObjectByType<PlayerAnimationBinder>();
            if (binder == null)
            {
                Debug.LogError("Target director was null", this);
                return false;
            }
            return true;
        }

        public void TriggerOnceAfterAnimation(Action action)
        {
            if (cutscene == null)
            {
                Debug.LogError("Cutscene is null", this);
                return;
            }
            afterAnimation.Add(action);
        }
        public void SearchForFields()
        {
            FindBinder();
            _gameStateManager = FindAnyObjectByType<GameStateManager>();
            director = binder.GetDirector(PlayerDirectorName);
            closingCutscene = binder.GetTimeline(PlayerTimelineName);

        }
        protected override bool NullHandling(PlayerFSM target)
        {
            if (target == null)
            {
                Debug.LogError("Target object was null", this);
                return false;
            }
            if (_gameStateManager == null)
            {
                _gameStateManager = FindAnyObjectByType<GameStateManager>();
                if (_gameStateManager == null)
                {
                    Debug.LogError("GameStateManager was null", this);
                    return false;
                }
            }
            if (!noArm)
            {
                if (director == null)
                {
                    if (!FindBinder()) return false;
                    director = binder.GetDirector(PlayerDirectorName);
                    if (director == null)
                    {
                        Debug.LogError("Target director was null", this);
                        return false;
                    }
                }
                if (closingCutscene == null)
                {
                    if (!FindBinder()) return false;
                    closingCutscene = binder.GetTimeline(PlayerTimelineName);
                    if (closingCutscene == null)
                    {
                        Debug.LogError("Asset was null", this);
                        return false;
                    }
                }
            }
            if (useRaycast && raycastTarget == null)
            {
                raycastTarget = door.GetComponentInChildren<Collider>();
                if (raycastTarget == null)
                {
                    Debug.LogError("Raycast target was null", this);
                    return false;
                }
            }
            return true;
        }
        public override void Trigger()
        {
            if (!door.isOpen)
                asset = openingCutscene;
            else
                asset = closingCutscene;

            if (!noArm)
            {
                base.Trigger();
                cutscene.totalDuration = (float)asset.duration;
            }
            cutscene.OnEnd.AddListener(OnEndInternal);
            cutscene.totalDuration = 1;
            cutscene.StartAnimation(door);
        }

        public void TriggerExternally()
        {
            SearchForFields();
            Trigger();
        }
        public void SetExternally(bool needsToBeOpen)
        {
            if (needsToBeOpen != door.isOpen)
            {
                TriggerExternally();
            }
        }
        void OnEndInternal()
        {
            AnimationEnd();
            OnAnimationEnd?.Invoke();
            cutscene.OnEnd.RemoveListener(OnEndInternal);

            Debug.Log("Timeline animation ended", this);
        }

        protected override void AnimationEnd()
        {
            base.AnimationEnd();
            Debug.Log($"{afterAnimation.Count} postAnim was triggered");
            foreach (var action in afterAnimation)
            {
                action();
            }
            afterAnimation.Clear();
        }

        public override void Unlock()
        {
            isLocked = false;
        }

        public void Lock()
        {
            isLocked = true;
        }
    }
}
