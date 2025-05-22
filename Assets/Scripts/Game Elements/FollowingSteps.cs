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
        [SerializeField] float speed;

        [SerializeField] Curve curve;

        Coroutine movement;
        public void StartFollowing()
        {
            movement = StartCoroutine(nameof(Step));
        }
        public void EndFollowing()
        {
            if (movement != null)
                StopCoroutine(movement);
        }

        IEnumerator Step()
        {
            float dist = curve.Getlength();
            float fac = 0;
            while (fac < 1)
            {
                fac += speed / dist * Time.deltaTime;
                transform.position = curve.GetPositionFromDistanceGS(fac * dist);
                yield return new WaitForEndOfFrame();
            }
            yield break;
        }

        public void Start()
        {
            transform.position = curve.startGS;
        }
    }
}
