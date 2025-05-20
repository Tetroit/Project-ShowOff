using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

namespace amogus
{
    public class FollowingSteps : MonoBehaviour
    {
        [SerializeField] Vector3 start;
        [SerializeField] Vector3 end;
        [SerializeField] float speed;

        Coroutine movement;
        public void StartFollowing()
        {
            movement = StartCoroutine(nameof(Step));
        }
        public void EndFollowing()
        {
            StopCoroutine(movement);
        }

        IEnumerator Step()
        {

            Vector3 d = end - start;
            float dist = d.magnitude;
            float fac = 0;
            while (fac < 1)
            {
                fac += speed / dist * Time.deltaTime;
                transform.position = Vector3.Lerp(start, end, fac);
                yield return new WaitForEndOfFrame();
            }
            yield break;
        }

        public void Start()
        {
            transform.position = start;
        }
    }
}
