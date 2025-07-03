using System;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.Rendering;

namespace amogus
{
    public enum InputDevice
    {
        Keyboard,
        Joystick
    }

    public enum MovementState
    {
        Walk = 0,
        Crouch = 1,
        Sprint = 2,
    }

    [RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
    public class FreePlayerController : PlayerController
    {
        [SerializeField] List<PhysicsControllerState> states = new List<PhysicsControllerState>();
        [SerializeField] MovementState currentState = 0;
        [SerializeField] private float walkFOV = 60f;
        [SerializeField] private float sprintFOV = 60f;
        public PhysicsControllerState state => states[(int)currentState];

        public float height
        {
            get => coll.height;
            set => coll.height = value;
        }
        public override bool isMoving => _isMoving;
        public bool _isMoving { get; private set; }
        bool _startedSprinting;
        bool _stopedSprinting;

        public bool isGrounded { get; private set; }

        public bool isSprinting { get; private set; }
        public bool isCrouching { get; private set; }
        public bool lockControls;

        bool needsCrouchHandling = false;

        public InputDevice inputDevice;

        List<ContactPoint> contacts = new List<ContactPoint>();

        Vector3 castOffset;
        float castRadius;

        [SerializeField] Transform cameraTransform;
        [SerializeField] bool _debug;
        [SerializeField] float _roofDetectionRange;

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
            if (!_debug) return;


            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(transform.position, transform.up * _roofDetectionRange);
            
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

        public void SetNewStates( PhysicsControllerState walk, PhysicsControllerState crouch, PhysicsControllerState sprint)
        {
            states[0] = walk;
            states[1] = crouch;
            states[2] = sprint;
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

            //----------------INPUT READ--------------------------

            if (!lockControls)
            {
                Vector3 forward = Vector3.ProjectOnPlane(cameraTransform.forward, Vector3.up).normalized;
                Vector3 up;

                //account for slope
                if (isGrounded)
                {
                    up = groundNormal;
                    forward = Vector3.ProjectOnPlane(forward, up);
                }
                Vector3 right = Vector3.Cross(forward, Vector3.down);

                moveDirection += PlayerInputHandler.Instance.Move.y * forward;
                moveDirection += PlayerInputHandler.Instance.Move.x * right;

                if (PlayerInputHandler.Instance.JumpPressed && isGrounded) shouldJump = true;


                if (isCrouching)
                {
                    if (needsCrouchHandling)
                    {
                        if (!Physics.Raycast(transform.position, transform.up, _roofDetectionRange,int.MaxValue,QueryTriggerInteraction.Ignore))
                            SwitchState(MovementState.Walk);
                        needsCrouchHandling = false;
                    }
                    else if (PlayerInputHandler.Instance.SprintPressed)
                    {
                        if (!Physics.Raycast(transform.position, transform.up, _roofDetectionRange))
                            SwitchState(MovementState.Sprint);
                    }
                }
                else
                {
                    if (needsCrouchHandling)
                    {
                        SwitchState(MovementState.Crouch);
                        needsCrouchHandling = false;
                    }
                    else if (PlayerInputHandler.Instance.SprintPressed)
                        SwitchState(MovementState.Sprint);
                    else
                        SwitchState(MovementState.Walk);
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

            if(_isMoving && currentState == MovementState.Sprint)
            {
                if(!_startedSprinting)
                {
                    OnFOVChange?.Invoke(sprintFOV);
                }
                _startedSprinting = true;
                _stopedSprinting = false;
            }
            else
            {
                if(!_stopedSprinting)
                {
                    OnFOVChange?.Invoke(walkFOV);

                }
                _startedSprinting = false;
                _stopedSprinting = true;

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

        public void SwitchState (MovementState newState)
        {
            if (currentState == newState) return;
            rb.position += new Vector3(0, (states[(int)newState].height - state.height) / 2f, 0);
            currentState = newState;
            isCrouching = newState == MovementState.Crouch;
            isSprinting = newState == MovementState.Sprint;

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

            if (_debug)
            Debug.Log("enabled free move controls");
        }

        public override void DisableControl()
        {
            rb.Sleep();
            rb.isKinematic = true;
            coll.enabled = false;
            lockControls = true;

            _isMoving = false;
            if(_debug)
            Debug.Log("disabled free move controls");
        }
    }

}
