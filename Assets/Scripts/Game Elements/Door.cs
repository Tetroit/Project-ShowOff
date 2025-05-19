using UnityEngine;

namespace amogus
{
    public class Door : MonoBehaviour
    {
        public string unlockCode;
        [field: SerializeField] public bool isOpen { get; private set; } = false;
        [SerializeField] Quaternion openRotation;
        [SerializeField] Quaternion closedRotation;

        public void OnEnable()
        {
            if (isOpen)
            {
                transform.rotation = openRotation;
            }
            else
            {
                transform.rotation = closedRotation;
            }
        }

        public Quaternion GetTargetRotation()
        {
            return isOpen ? closedRotation : openRotation;
        }
        public Quaternion GetStartRotation()
        {
            return isOpen ? openRotation : closedRotation;
        }

        public void Open()
        {
            isOpen = true;
            transform.rotation = openRotation;
        }

        public void Close()
        {
            isOpen = false;
            transform.rotation = closedRotation;
        }
    }
}
