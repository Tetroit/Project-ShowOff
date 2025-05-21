using UnityEngine;

namespace amogus
{
    public class Door : MonoBehaviour
    {
        [field: SerializeField] public bool isOpen { get; private set; } = false;
        [SerializeField] Quaternion openRotation;
        [SerializeField] Quaternion closedRotation;

        public void OnEnable()
        {
            if (isOpen)
            {
                Open();
            }
            else
            {
                Close();
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
            transform.localRotation = openRotation;
        }

        public void Close()
        {
            isOpen = false;
            transform.localRotation = closedRotation;
        }
    }
}
