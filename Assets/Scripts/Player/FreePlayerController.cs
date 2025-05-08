using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;

namespace amogus
{
    public enum InputDevice
    {
        Keyboard,
        Joystick
    }
    public class FreePlayerController : PlayerController
    {
        [System.Serializable]
        public struct PlayerState
        {
            public bool lockControls;
            public float movementSpeed;
            public float speedLimit;
            public float airSpeed;
            public float criticalAngle;
            public float jumpHeight;
            public float acceleration;


            public PlayerState(
                bool lockControls = false,
                float movementSpeed = 5f,
                float speedLimit = 10f,
                float airSpeed = 1f,
                float criticalAngle = 30f,
                float jumpHeight = 5f,
                float acceleration = 1f
            )
            {
                this.lockControls = lockControls;
                this.movementSpeed = movementSpeed;
                this.speedLimit = speedLimit;
                this.airSpeed = airSpeed;
                this.criticalAngle = criticalAngle;
                this.jumpHeight = jumpHeight;
                this.acceleration = acceleration;
            }
        }
        [SerializeField] PlayerState state = new PlayerState();

        public bool isMoving { get; private set; }
        public bool isGrounded { get; private set; }
        bool isSafe;
        float accelerationFac = 0;

        Vector3 lastSafePos;

        public InputDevice inputDevice;

        List<ContactPoint> contacts = new List<ContactPoint>();

        float turnX;

        Vector3 castOffset;
        float castRadius;

        [SerializeField] Transform cameraTransform;
        Rigidbody rb;
        Collider coll;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            coll = GetComponent<Collider>();
        }
        private void Start()
        {

            isSafe = true;

            if (cameraTransform != null)
            {
                cameraTransform.localRotation = transform.rotation;
            }

        }

        private void LateUpdate()
        {
            if (state.lockControls) return;
            LookAround();
            cameraTransform?.GetComponent<PlayerCamera>().UpdateTransform(transform.position, turnX);
            //timeSinceFixedUpdate = Time.time - lastFixedUpdateTime;
        }
        private void FixedUpdate()
        {
            if (state.lockControls) return;
            Move();
            contacts.Clear();
        }
        private void OnCollisionStay(Collision collision)
        {
            //List<ContactPoint> currentContacts = new List<ContactPoint>();
            List<ContactPoint> currentContacts = new List<ContactPoint>();
            collision.GetContacts(currentContacts);
            contacts.AddRange(currentContacts);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(transform.position + castOffset, castRadius);

            foreach (var contact in contacts)
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawLine(contact.point, contact.point + contact.normal);
            }
        }


        private void LookAround()
        {
            turnX += Input.GetAxis("Mouse X");
            //if (cameraTransform)
            //    cameraTransform.localRotation = Quaternion.Euler(-turn.y, 0f, 0f);

        }

        private void Move()
        {
            Vector3 moveDirection = Vector3.zero;
            bool shouldJump = false;
            isGrounded = false;
            List<Vector3> constraints = new List<Vector3>();

            //----------------GROUND CHECK-------------------------

            foreach (ContactPoint c in contacts)
            {
                float flatness = c.normal.y;
                if (flatness > Mathf.Cos(state.criticalAngle * Mathf.Deg2Rad))
                    isGrounded = true;

                Debug.Log(flatness + " <----> " + state.criticalAngle);
            }
            //if (!isGrounded)
            //{
            //    RaycastHit hit;
            //    if (Physics.SphereCast(rb.position + castOffset, castRadius, Vector3.down, out hit))
            //    {
            //        if (hit.distance < castRadius ) isGrounded = true;
            //        Debug.Log(hit.collider.name);
            //    }
            //}

            //----------------INPUT READ--------------------------

            if (!state.lockControls)
            {
                if (inputDevice == InputDevice.Joystick)
                {
                    moveDirection += Input.GetAxisRaw("Vertical") * transform.forward;
                    moveDirection += Input.GetAxisRaw("Horizontal") * transform.right;
                }
                if (inputDevice == InputDevice.Keyboard)
                {
                    if (Input.GetKey(KeyCode.W)) moveDirection += transform.forward;
                    if (Input.GetKey(KeyCode.S)) moveDirection -= transform.forward;
                    if (Input.GetKey(KeyCode.D)) moveDirection += transform.right;
                    if (Input.GetKey(KeyCode.A)) moveDirection -= transform.right;

                    if (Input.GetKey(KeyCode.Space) && isGrounded) shouldJump = true;
                }
            }

            if (moveDirection.magnitude > 0.1f)
            {
                isMoving = true;
            }
            else
            {
                isMoving = false;
            }

            //-------------COLLISION RESOLUTION-------------

            moveDirection = moveDirection.normalized * state.movementSpeed;

            Vector3 rbCopy;
            if (isGrounded)
            {
                rbCopy = new Vector3(moveDirection.x, rb.linearVelocity.y, moveDirection.z);
            }
            else
                rbCopy = rb.linearVelocity + new Vector3(moveDirection.x * Time.deltaTime * state.airSpeed, 0, moveDirection.z * Time.deltaTime * state.airSpeed);

            if (!isMoving)
            {
                rbCopy -= moveDirection.normalized * state.acceleration * Time.deltaTime;
            }

            if (rbCopy.magnitude > state.speedLimit)
                rbCopy = rbCopy.normalized * state.speedLimit;

            foreach (ContactPoint c in contacts)
            {
                float dot = Vector3.Dot(c.normal, rbCopy);
                if (dot < 0f)
                {
                    constraints.Add(c.normal);
                }
            }

            constraints.Add(rbCopy);

            Orhtogonalise(constraints);

            rb.linearVelocity = rbCopy;

            rb.rotation = Quaternion.Euler(0f, turnX, 0f);
            //rb.velocity = constraints[constraints.Count - 1];


            // -----------------JUMP-----------------
            if (shouldJump)
            {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x/2, state.jumpHeight, rb.linearVelocity.z/2);
            }
        }

        //to learn more about the thing down here google "Gram Schmidt process"
        public void Orhtogonalise(List<Vector3> vecs)
        {
            GizmoManager.Instance.Clear(transform);
            GizmoManager.Instance.StageLine(Vector3.zero, vecs[0], new Color(0f, 1f, 0f), transform);
            for (int i = 1; i < vecs.Count; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    vecs[i] -= Vector3.Project(vecs[i], vecs[j]);
                }
                GizmoManager.Instance.StageLine(Vector3.zero, vecs[i], new Color(0.4f * i, 1f, 0f), transform);
            }
        }

        public override void EnableControl()
        {
            rb.position = cameraTransform.position;
            turnX = cameraTransform.rotation.eulerAngles.y;
            rb.isKinematic = false;
            coll.enabled = true;
            state.lockControls = false;
        }

        public override void DisableControl()
        {
            rb.isKinematic = true;
            coll.enabled = false;
            state.lockControls = true;
        }
    }

}
