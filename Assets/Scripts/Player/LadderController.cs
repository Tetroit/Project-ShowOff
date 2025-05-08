using amogus;
using UnityEngine;

namespace amogus
{
    public class LadderController : PlayerController
    {
        [SerializeField]
        Transform target;

        public override void DisableControl()
        {
        }

        public override void EnableControl()
        {
        }

        public void Update()
        {
            if (Input.GetKey(KeyCode.W))
            {
                target.transform.position += Vector3.up * Time.deltaTime * 3;
            }
            if (Input.GetKey(KeyCode.S))
            {
                target.transform.position -= Vector3.up * Time.deltaTime * 3;
            }
        }
    }
}

