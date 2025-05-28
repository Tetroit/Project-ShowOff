using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

namespace amogus
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class MillerController : MonoBehaviour
    {
        [SerializeField] float _destinationUpdateSpeed;
        [SerializeField] Animator _anim;

        [Header("For testing")]
        [SerializeField] Transform _testTarget;
        [SerializeField] bool _test;
        [SerializeField] bool _stopTest;

        NavMeshAgent _agent;
        Coroutine _followBehavior;

        void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
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
            _anim.Play("Walk");
        }

        public void StopFollowing()
        {
            if (_followBehavior == null) return;
            StopCoroutine(_followBehavior);
            _followBehavior = null;
            _anim.Play("Idle");
        }

        IEnumerator Follow(Transform target)
        {
            WaitForSeconds wait = new(_destinationUpdateSpeed);
            while(true)
            {
                _agent.SetDestination(target.position);
                yield return wait;

            }
        }
    }
}
