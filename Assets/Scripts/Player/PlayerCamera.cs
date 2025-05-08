using System.Data;
using UnityEngine;

namespace amogus
{
    public class PlayerCamera : MonoBehaviour
    {
        [SerializeField] Vector3 offset;
        [SerializeField] float minAngle = -20f;
        [SerializeField] float maxAngle = 35f;

        public bool isCinematic;
        Quaternion targetRotation;
        Vector2 turn;

        [Range(0f, 1f)]
        public float cameraSmoothness = 0.5f;
        public float movementSmoothness = 0.5f;
        public void UpdateTransform(Vector3 pos, float hDir)
        {
            transform.position = Vector3.Lerp(transform.position, pos + offset, movementSmoothness);
            //turn.x = hDir;
        }

        private void Start()
        {
            ReadRotation();
        }
        public void ReadRotation()
        {
            turn.x = transform.rotation.eulerAngles.y;
            turn.y = -transform.rotation.eulerAngles.x;
        }
        private void LateUpdate()
        {
            if (!isCinematic) 
            {
                turn += new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
                turn.y = Mathf.Clamp(turn.y, minAngle, maxAngle);
                targetRotation = Quaternion.Euler(-turn.y, turn.x, 0);
                transform.rotation = Quaternion.Slerp(targetRotation, transform.rotation, cameraSmoothness);
            }
        }

    }
}
