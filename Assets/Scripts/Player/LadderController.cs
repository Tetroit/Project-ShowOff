using amogus;
using UnityEngine;

namespace amogus
{
    public class LadderController : PlayerController
    {
        [SerializeField] Transform target; 
        [SerializeField] float speed;

        Ladder ladderContext;
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
            target.transform.position += Time.deltaTime * speed * PlayerInputHandler.Instance.Move.y * Vector3.up;
        }

        public void SetLadder(Ladder ladder)
        {
            ladderContext = ladder;
        }
    }
}

