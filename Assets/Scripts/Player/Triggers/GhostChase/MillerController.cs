using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace amogus
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class MillerController : MonoBehaviour
    {
        [SerializeField] float _destinationUpdateSpeed;
        NavMeshAgent _agent;
        Coroutine _followBehavior;

        public void StartFollowing(Transform target)
        {
            _followBehavior = StartCoroutine(Follow(target));
        }

        public void StopFollowing()
        {
            StopCoroutine(_followBehavior);
            _followBehavior = null;
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
