using FMOD.Studio;
using FMODUnity;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace amogus
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class MillerController : MonoBehaviour
    {
        [SerializeField] float _destinationUpdateSpeed;
        [SerializeField] Animator _anim;
        [SerializeField] GameStateManager _gameStateManager;
        [SerializeField] float _followWait;
 
        [Header("For testing")]
        [SerializeField] Transform _testTarget;
        [SerializeField] bool _test;
        [SerializeField] bool _stopTest;

        EventInstance followMusicEvent;

        NavMeshAgent _agent;
        Coroutine _followBehavior;

        void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _anim.Play("Idle");

        }

        private void Update()
        {
            if(_test)
            {
                _test = false;
                StartFollowing(_testTarget);
            }

            if (_stopTest)
            {
                _stopTest = false;
                StopFollowing();
            }
        }

        public void StartFollowing(Transform target)
        {
            _followBehavior = StartCoroutine(Follow(target));
            
        }

        public void StopFollowing()
        {
            if (_followBehavior == null) return;
            StopCoroutine(_followBehavior);
            _followBehavior = null;
            _anim.Play("Idle");
            followMusicEvent.setParameterByName("FadeOut", 1);
        }

        IEnumerator Follow(Transform target)
        {
            followMusicEvent = RuntimeManager.CreateInstance(FMODEvents.instance.followMusic);
            followMusicEvent.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
            followMusicEvent.start();
            yield return new WaitForSeconds(_followWait);
            _anim.Play("Walk");
            WaitForSeconds wait = new(_destinationUpdateSpeed);
            while(true)
            {
                _agent.SetDestination(target.position);
                yield return wait;

            }
        }

        private void OnDisable()
        {
            followMusicEvent.setParameterByName("FadeOut", 1);
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer != LayerMask.NameToLayer("Player")) return;

            _gameStateManager.SwitchToGameOver();
        }
    }
}
