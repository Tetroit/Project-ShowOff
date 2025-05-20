using System;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;
using UnityEngine.InputSystem;
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
        [SerializeField] List<PhysicsControllerState> states = new List<PhysicsControllerState>();
        [SerializeField] int currentState = 0;
        public PhysicsControllerState state => states[currentState];

        public float height
        {
            get => coll.height;
            set => coll.height = value;
        }
        public override bool isMoving => _isMoving;
        public bool _isMoving { get; private set; }
        public bool isGrounded { get; private set; }

        public bool isSprinting { get; private set; }
        public bool isCrouching { get; private set; }
        bool isSafe;
        public bool lockControls;
        float accelerationFac = 0;

        bool needsCrouchHandling = false;

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

        private void OnEnable()
        {
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
            cameraTransform.GetComponent<PlayerCamera>().UpdateTransform(transform.position);
            if (PlayerInputHandler.Instance.CrouchPressedThisFrame)
            {
                needsCrouchHandling = true;
            }
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
            GizmoManager.Instance.Clear(transform);
            Vector3 moveDirection = Vector3.zero;
            bool shouldJump = false;
            isGrounded = false;
            List<Vector3> constraints = new List<Vector3>();

            //----------------GROUND CHECK-------------------------

            Vector3 groundNormal = Vector3.up;
            foreach (ContactPoint c in contacts)
            {
                float flatness = c.normal.y;
                if (flatness > Mathf.Cos(state.criticalAngle * Mathf.Deg2Rad))
                {
                    groundNormal = c.normal;
                    isGrounded = true;
                }

                //Debug.Log(flatness + " <----> " + state.criticalAngle);
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
                Vector3 forward = Vector3.ProjectOnPlane(cameraTransform.forward, Vector3.up).normalized;
                Vector3 up = Vector3.up;
                //account for slope
                if (isGrounded)
                {
                    up = groundNormal;
                    forward = Vector3.ProjectOnPlane(forward, up);
                }
                Vector3 right = Vector3.Cross(forward, Vector3.down);

                //if (inputDevice == InputDevice.Joystick)
                //{
                //    moveDirection += Input.GetAxisRaw("Vertical") * forward;
                //    moveDirection += Input.GetAxisRaw("Horizontal") * right;
                //}
                //if (inputDevice == InputDevice.Keyboard)
                //{
                //    //movement
                //    if (Input.GetKey(KeyCode.W)) moveDirection += forward;
                //    if (Input.GetKey(KeyCode.S)) moveDirection -= forward;
                //    if (Input.GetKey(KeyCode.D)) moveDirection += right;
                //    if (Input.GetKey(KeyCode.A)) moveDirection -= right;

                //    GizmoManager.Instance.StageLine(Vector3.zero, moveDirection * 10, Color.cyan, transform);
                //    //jump
                //    if (Input.GetKey(KeyCode.Space) && isGrounded) shouldJump = true;

                //    //states
                //    if (Input.GetKey(KeyCode.LeftShift))
                //        SwitchState(1);
                //    else if (Input.GetKey(KeyCode.LeftControl))
                //        SwitchState(2);
                //    else 
                //        SwitchState(0);
                //};

                moveDirection += PlayerInputHandler.Instance.Move.y * forward;
                moveDirection += PlayerInputHandler.Instance.Move.x * right;

                if (PlayerInputHandler.Instance.JumpPressed && isGrounded) shouldJump = true;


                if (isCrouching)
                {
                    if (needsCrouchHandling)
                    {
                        SwitchState(0);
                        needsCrouchHandling = false;
                    }
                    else if (PlayerInputHandler.Instance.SprintPressed)
                        SwitchState(2);
                }
                else
                {
                    if (needsCrouchHandling)
                    {
                        SwitchState(1);
                        needsCrouchHandling = false;
                    }
                    else if (PlayerInputHandler.Instance.SprintPressed)
                        SwitchState(2);
                    else
                        SwitchState(0);
                }
            }
            //-------------STATE RESOLUTION-------------

            if (moveDirection.magnitude > 0.1f)
            {
                _isMoving = true;
            }
            else
            {
                _isMoving = false;
            }

            moveDirection = moveDirection.normalized * state.movementSpeed;
            Vector3 rbCopy;
            if (isGrounded)
            {
                rbCopy = new Vector3(moveDirection.x, moveDirection.y, moveDirection.z);
                rb.useGravity = false;
            }
            else
            {
                rbCopy = rb.linearVelocity + new Vector3(moveDirection.x * state.airSpeed, 0, moveDirection.z * state.airSpeed);
                rb.useGravity = true;
            }

            //-------------COLLISION RESOLUTION-------------



            //if (!isMoving)
            //{
            //    rbCopy -= moveDirection.normalized * state.acceleration * Time.deltaTime;
            //}

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

            GizmoManager.Instance.StageLine(Vector3.zero, rb.linearVelocity, Color.blue, transform);

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
            for (int i = 1; i < vecs.Count; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    vecs[i] -= Vector3.Project(vecs[i], vecs[j]);
                }
            }
        }


        public void SwitchState (int newState)
        {
            if (currentState == newState) return;
            rb.position += new Vector3(0, (states[newState].height - state.height) / 2f, 0);
            currentState = newState;
            isCrouching = newState == 1;
            isSprinting = newState == 2;

            OnCameraShakeChange?.Invoke((CameraWalkingShake.State)newState);
        }
        public override void EnableControl()
        {
            OnCameraShakeChange?.Invoke(CameraWalkingShake.State.WALKING);
            transform.position = cameraTransform.position;
            rb.position = cameraTransform.position;
            coll.enabled = true;
            lockControls = false;
            rb.isKinematic = false;
            rb.WakeUp();

            Debug.Log("enabled free move controls");
        }

        public override void DisableControl()
        {
            rb.Sleep();
            rb.isKinematic = true;
            coll.enabled = false;
            lockControls = true;

            _isMoving = false;

            Debug.Log("disabled free move controls");
        }
    }

}
