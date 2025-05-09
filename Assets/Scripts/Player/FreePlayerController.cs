using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;
using UnityEngine.Rendering;

namespace amogus
{
    public enum InputDevice
    {
        Keyboard,
        Joystick
    }
    [RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
    public class FreePlayerController : PlayerController
    {
        [System.Serializable]
        public struct PlayerState
        {
            public string name;
            public float movementSpeed;
            public float speedLimit;
            public float airSpeed;
            public float criticalAngle;
            public float jumpHeight;
            public float acceleration;
            public float height;


            public PlayerState(
                string name = "lobotomy",
                float movementSpeed = 5f,
                float speedLimit = 10f,
                float airSpeed = 1f,
                float criticalAngle = 30f,
                float jumpHeight = 5f,
                float acceleration = 1f,
                float height = 2f
            )
            {
                this.name = name;
                this.movementSpeed = movementSpeed;
                this.speedLimit = speedLimit;
                this.airSpeed = airSpeed;
                this.criticalAngle = criticalAngle;
                this.jumpHeight = jumpHeight;
                this.acceleration = acceleration;
                this.height = height;
            }

            public static PlayerState Lerp (PlayerState s1,  PlayerState s2, float fac)
            {
                return new PlayerState(
                    $"lerp between {s1.name} and {s2.name}",
                    Mathf.Lerp(s1.movementSpeed, s2.movementSpeed, fac),
                    Mathf.Lerp(s1.speedLimit, s2.speedLimit, fac),
                    Mathf.Lerp(s1.airSpeed, s2.airSpeed, fac),
                    Mathf.Lerp(s1.criticalAngle, s2.criticalAngle, fac),
                    Mathf.Lerp(s1.jumpHeight, s2.jumpHeight, fac),
                    Mathf.Lerp(s1.acceleration, s2.acceleration, fac),
                    Mathf.Lerp(s1.height, s2.height, fac)
                    );
            }

            public override string ToString()
            {
                return name;
            }
        }

        [SerializeField] List<PlayerState> states = new List<PlayerState>();
        [SerializeField] int currentState = 0;
        public PlayerState state => states[currentState];

        public float height
        {
            get => coll.height;
            set => coll.height = value;
        }
        public bool isMoving { get; private set; }
        public bool isGrounded { get; private set; }

        public bool isSprinting { get; private set; }
        public bool isCrouching { get; private set; }
        bool isSafe;
        public bool lockControls;
        float accelerationFac = 0;

        Vector3 lastSafePos;

        public InputDevice inputDevice;

        List<ContactPoint> contacts = new List<ContactPoint>();

        Vector3 castOffset;
        float castRadius;

        [SerializeField] Transform cameraTransform;
        Rigidbody rb;
        CapsuleCollider coll;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            coll = GetComponent<CapsuleCollider>();
        }
        private void Start()
        {

            isSafe = true;

            if (cameraTransform != null)
            {
                cameraTransform.localRotation = transform.rotation;
            }

        }

        private void Update()
        {
            if (lockControls) return;
            cameraTransform?.GetComponent<PlayerCamera>().UpdateTransform(transform.position);
        }
        private void LateUpdate()
        {
            if (lockControls) return;
            LookAround();
            //timeSinceFixedUpdate = Time.time - lastFixedUpdateTime;
        }
        private void FixedUpdate()
        {
            if (lockControls) return;
            height = state.height;
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
            //if (cameraTransform)
            //    cameraTransform.localRotation = Quaternion.Euler(-turn.y, 0f, 0f);

        }

        public Quaternion GetCameraRotation()
        {
            return cameraTransform.rotation;
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

            if (!lockControls)
            {
                var up = Vector3.up;
                var forward = Vector3.ProjectOnPlane(cameraTransform.forward, Vector3.up);
                var right = Vector3.Cross(forward, Vector3.down);
                if (inputDevice == InputDevice.Joystick)
                {
                    moveDirection += Input.GetAxisRaw("Vertical") * forward;
                    moveDirection += Input.GetAxisRaw("Horizontal") * transform.right;
                }
                if (inputDevice == InputDevice.Keyboard)
                {
                    //movement
                    if (Input.GetKey(KeyCode.W)) moveDirection += forward;
                    if (Input.GetKey(KeyCode.S)) moveDirection -= forward;
                    if (Input.GetKey(KeyCode.D)) moveDirection += right;
                    if (Input.GetKey(KeyCode.A)) moveDirection -= right;

                    GizmoManager.Instance.StageLine(Vector3.zero, moveDirection * 10, Color.cyan, transform);
                    //jump
                    if (Input.GetKey(KeyCode.Space) && isGrounded) shouldJump = true;

                    //states
                    if (Input.GetKey(KeyCode.LeftShift))
                        SwitchState(1);
                    else if (Input.GetKey(KeyCode.LeftControl))
                        SwitchState(2);
                    else 
                        SwitchState(0);
                };
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

            rb.rotation = Quaternion.Euler(0f, GetCameraRotation().eulerAngles.x, 0f);
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


        public void SwitchState (int newState)
        {
            rb.position += new Vector3(0, (states[newState].height - state.height) / 2f, 0);
            currentState = newState;
        }
        public override void EnableControl()
        {
            transform.position = cameraTransform.position;
            rb.position = cameraTransform.position;
            rb.linearVelocity = Vector3.zero;
            turnX = cameraTransform.rotation.eulerAngles.y;
            coll.enabled = true;
            state.lockControls = false;
            rb.isKinematic = false;
            rb.WakeUp();

            Debug.Log("enabled free move controls");
        }

        public override void DisableControl()
        {
            rb.Sleep();
            rb.isKinematic = true;
            coll.enabled = false;
            state.lockControls = true;

            Debug.Log("disabled free move controls");
        }
    }

}
