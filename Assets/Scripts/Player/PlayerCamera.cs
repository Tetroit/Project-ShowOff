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
        Camera cam;

        public float targetFOV = 60;
        public float currentFOV { 
            get => cam.fieldOfView;
        }
        [SerializeField] Transform xRotator;
        [SerializeField] Transform yRotator;
        [SerializeField] Transform Movement;

        [SerializeField] float fovSpeed = 3;

        [Range(0f, 1f)]
        public float cameraSmoothness = 0.5f;
        [Range(0f, 1f)]
        public float movementSmoothness = 0.5f;
        public void UpdateTransform(Vector3 pos)
        {
            Movement.position = Vector3.Lerp(pos, Movement.position, movementSmoothness);
            //turn.x = hDir;
        }

        private void Start()
        {
            cam = GetComponentInChildren<Camera>();
            if (cam == null)
                Debug.Log("Camera not found");
            else
                targetFOV = cam.fieldOfView;
            ReadRotation();
        }
        public void SetViewRotation(Vector2 rotation)
        {
            if (xRotator == yRotator)
            {
                xRotator.localRotation = Quaternion.Euler(-rotation.y, rotation.x, 0);
            }
            else
            {
                xRotator.localRotation = Quaternion.Euler(-rotation.y, 0, 0);
                yRotator.localRotation = Quaternion.Euler(0, rotation.x, 0);
            }
        }
        public void ReadRotation()
        {
            turn.x = yRotator.rotation.eulerAngles.y;
            turn.y = -yRotator.rotation.eulerAngles.x;

            if (turn.y < -90)
                turn.y += 360;
            if (turn.y > 90)
                turn.y -= 360;
        }
        private void Update()
        {
            cam.fieldOfView = Mathf.Lerp(currentFOV, targetFOV, Time.deltaTime * fovSpeed);
        }
        private void LateUpdate()
        {
            if (!isCinematic) 
            {
                turn += PlayerInputHandler.Instance.View * 0.2f; //sensitivity here, 0.2 meanwhile
                turn.y = Mathf.Clamp(turn.y, minAngle, maxAngle);
                if (xRotator == yRotator)
                {
                    Quaternion targetRotation = Quaternion.Euler(-turn.y, turn.x, 0);
                    xRotator.localRotation = Quaternion.Slerp(targetRotation, xRotator.localRotation, cameraSmoothness);
                }
                else
                {
                    Quaternion targetYRotation = Quaternion.Euler(-turn.y, 0, 0);
                    Quaternion targetXRotation = Quaternion.Euler(0, turn.x, 0);
                    xRotator.localRotation = Quaternion.Slerp(targetXRotation, xRotator.localRotation, cameraSmoothness);
                    yRotator.localRotation = Quaternion.Slerp(targetYRotation, yRotator.localRotation, cameraSmoothness);
                }
            }
        }

        public void ChangeFOV(float FOV)
        {
            if (cam == null) return;
            targetFOV = FOV;
        }
    }
}
