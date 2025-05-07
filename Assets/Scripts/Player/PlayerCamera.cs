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

        [SerializeField] Transform xRotator;
        [SerializeField] Transform yRotator;
        [SerializeField] Transform Movement;
        [Range(0f, 1f)]
        public float cameraSmoothness = 0.5f;
        [Range(0f, 1f)]
        public float movementSmoothness = 0.5f;
        public void UpdateTransform(Vector3 pos)
        {
            Debug.Log(pos - Movement.position);
            Movement.position = Vector3.Lerp(pos, Movement.position, movementSmoothness);
            //turn.x = hDir;
        }

        private void Start()
        {
            ReadRotation();
        }
        public void ReadRotation()
        {
            turn.x = xRotator.rotation.eulerAngles.y;
            turn.y = -yRotator.rotation.eulerAngles.x;
        }
        private void LateUpdate()
        {
            if (!isCinematic) 
            {
                turn += new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
                turn.y = Mathf.Clamp(turn.y, minAngle, maxAngle);
                if (xRotator == yRotator)
                {
                    Quaternion targetRotation = Quaternion.Euler(-turn.y, turn.x, 0);
                    xRotator.localRotation = Quaternion.Slerp(targetRotation, xRotator.localRotation, cameraSmoothness);
                }
                else
                {
                    Debug.Log("GFEIB");
                    Quaternion targetYRotation = Quaternion.Euler(-turn.y, 0, 0);
                    Quaternion targetXRotation = Quaternion.Euler(0, turn.x, 0);
                    xRotator.localRotation = Quaternion.Slerp(targetXRotation, xRotator.localRotation, cameraSmoothness);
                    yRotator.localRotation = Quaternion.Slerp(targetYRotation, yRotator.localRotation, cameraSmoothness);
                }
            }
        }

    }
}
