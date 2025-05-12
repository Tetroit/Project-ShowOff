using amogus;
using UnityEngine;

namespace amogus
{
    public class LadderController : PlayerController
    {
        [SerializeField] Transform target; 
        [SerializeField] float speed;
        public override void DisableControl()
        {
            enabled = false;
            Debug.Log("ladder controls disabled");
        }

        public override void EnableControl()
        {
            enabled = true;
            Debug.Log("ladder controls enabled");
        }

        public void Update()
        {
            if (Input.GetKey(KeyCode.W))
            {
                target.transform.position += Time.deltaTime * speed * Vector3.up;
            }
            if (Input.GetKey(KeyCode.S))
            {
                target.transform.position -= Time.deltaTime * speed * Vector3.up;
            }
        }
    }
}

