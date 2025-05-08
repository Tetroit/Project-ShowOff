using amogus;
using UnityEngine;

namespace amogus
{
    [RequireComponent(typeof(Collider))]
    public class ControllerSwitch : MonoBehaviour
    {
        public PlayerFSM.ControllerType fromType;
        public PlayerFSM.ControllerType toType;
        public PlayerAnimation transition = null;
    }
}
