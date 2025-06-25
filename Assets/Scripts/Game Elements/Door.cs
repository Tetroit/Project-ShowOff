using FMODUnity;
using UnityEngine;

namespace amogus
{
    public class Door : MonoBehaviour
    {
        [field: SerializeField] public bool isOpen { get; private set; } = false;
        [SerializeField] Quaternion openRotation;
        [SerializeField] Quaternion closedRotation;
        [SerializeField] EventReference doorOpenSFX;
        [SerializeField] EventReference doorCloseSFX;
        [SerializeField] EventReference doorUnlockSFX;
        [SerializeField] EventReference unlockAttemptSFX;
        [SerializeField] private GameObject handle;

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

        public void PlayOpenSound()
        {
            if (AudioManager.instance != null)
            {
                AudioManager.instance.PlayOneShot(doorOpenSFX, transform.position);
            }
        }

        public void PlayCloseSound()
        {
            if (AudioManager.instance != null)
            {
                AudioManager.instance.PlayOneShot(doorCloseSFX, handle.transform.position);
            }
        }

        public void PlayUnlockSound()
        {
            if(AudioManager.instance != null)
            {
                AudioManager.instance.PlayOneShot(doorUnlockSFX, handle.transform.position);
            }
        }

        public void PlayAttemptUnlcokSound()
        {
            if (AudioManager.instance != null)
            {
                AudioManager.instance.PlayOneShot(doorUnlockSFX, handle.transform.position);
            }
        }
    }
}
