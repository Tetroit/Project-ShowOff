using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[SelectionBase]
public class LegManager : MonoBehaviour
{
    public bool IsMoving { get; private set; }
    [SerializeField] bool _move;
    [field: SerializeField] public bool Gizmos { get; private set; } = false;

    [SerializeField] Curve _path;
    [SerializeField] float _nextNodeActivationDistance = .5f;
    [SerializeField] LayerMask _groundMask;
    [SerializeField] float _pathLerpTime = 10;
    [SerializeField] float _upLerpTime = 10;
    [field: SerializeField] public float AbovePointHeight { get; private set; } = 1f;
    [field: SerializeField] public float TouchRaySize { get; private set; } = 1f;


    [SerializeField, Range(1, 20)] int _jointCount = 3;
    [Tooltip("How close can leg joints")]
    [field: SerializeField] public float AcceptableDistance { get; private set; } = .05f;
    [Tooltip("How many attempts to connect legs to ground before giving up")]
    [field: SerializeField] public int CalibrationAttempts { get; private set; } = 5;
    [Header("Leg positioning")]
    [field: SerializeField] public float AngleX { get; private set; } = .31f;
    [field: SerializeField] public float AngleY { get; private set; } = .2f;
    [field: SerializeField] public float DistanceFromBody { get; private set; } = .5f;


    [field: SerializeField] public float WobbleSpeed { get; private set; } = 1.69f;
    [field: SerializeField] public float WobbleAmplitude { get; private set; } = .15f;
    [field: SerializeField] public float ForwardReach { get; private set; } = .8f;
    [field: SerializeField] public float StepSpeed { get; private set; } = .5f;
    [field: SerializeField] public float StepDistance { get; private set; } = 1f;
    [field: SerializeField] public float RestStepDistance { get; private set; } = .1f;
    [Header("Move Variance")]
    [field: SerializeField] public float MoveVarianceSpeed { get; private set; } = 5;
    [field: SerializeField] public float MoveSpeed { get; private set; } = 5;
    [Tooltip("How fast and slow can the spider Move")]
    [SerializeField] AnimationCurve _varianceRange;

    [Header("Bodyheight related")]
    [Tooltip("Smoothing of changing body height")]
    [SerializeField] float _heightChangeLerp;
    [field: SerializeField] public float GroundOffset { get; private set; } = .8f;
    [SerializeField] bool _useGeneralHeightDetection;

    [Header("Leg models")]
    [SerializeField] BoxCollider _legPrefab;
    [SerializeField] BoxCollider firstLeg;
    [SerializeField] BoxCollider firstSecondLeg;

    Leg[] _legs;
    public bool Step;
    float _lastBodyHeight;
    bool _startedMoving;
    bool _waitForFirstLegs;
    int _currentPathNode;
    float _currMoveVal;
    readonly Collider[] _contactPoints = new Collider[10];
    Vector3 _lastUp;

    //for debug
    Vector3 _closestContactPoint;

    void Start()
    {
        _lastBodyHeight = transform.position.y;
        _lastUp = transform.up;
        _legs = new Leg[]
        {
           new (AngleX, 0, firstLeg, _jointCount, this                    , ForwardReach, _groundMask),
           new (AngleX, -AngleY, _legPrefab, _jointCount, this            , ForwardReach, _groundMask),
           new (AngleX, -AngleY*2, _legPrefab, _jointCount, this          , ForwardReach, _groundMask),

           new (AngleX, -AngleY*2 - 135, firstSecondLeg, _jointCount, this, ForwardReach, _groundMask),
           new (AngleX, -AngleY-135, _legPrefab, _jointCount, this        , ForwardReach, _groundMask),
           new (AngleX, - 135, _legPrefab, _jointCount, this              , ForwardReach, _groundMask),

        };

        SetAdjacentLegs(_legs);
    }

    void SetAdjacentLegs(Leg[] arr)
    {
        Debug.Assert(arr.Length % 2 == 0);

        int legsOnSide = (int)(arr.Length * .5f);
        for (int i = 0; i < arr.Length; i++)//attaches left right up down neighbours but assumes that the grid has only two rows
        {
            int left = i - 1;
            int right = i + 1;
            int up = i - legsOnSide;
            int down = i + legsOnSide;
            bool firstRow = i < legsOnSide;

            List<Leg> adjLegs = new(3);
            if (firstRow)//there are no up neighbours
            {
                if (left > -1) adjLegs.Add(arr[left]);
                if (right < legsOnSide) adjLegs.Add(arr[right]);
                adjLegs.Add(arr[down]);
            }
            else//there are no down neighbours
            {
                if (left > legsOnSide - 1) adjLegs.Add(arr[left]);
                if (right < arr.Length) adjLegs.Add(arr[right]);
                adjLegs.Add(arr[up]);
            }

            arr[i].SetAdjacentLegs(adjLegs.ToArray());
        }
    }

    void Update()
    {
        if (_move)
        {
            SetUpFirstStep();
            if (_waitForFirstLegs)
                MoveBody();
        }
        else
        {
            _waitForFirstLegs = false;
            _startedMoving = false;
        }

        IsMoving = _move;
        UpdateLegs();
        if (_useGeneralHeightDetection)
            SetBodyHeightGeneral();
        else
            SetBodyHeight();
    }

    void UpdateLegs()
    {
        foreach (Leg leg in _legs)
            leg.Update();
    }

    void GetCurrentNode()
    {
        if (_currentPathNode == _path.points.Count) return;

        Vector3 currentTarget = _path.points[_currentPathNode];

        bool nodeIsBehind = Vector3.Distance(currentTarget, transform.position) < _nextNodeActivationDistance * GetSize();
        if (nodeIsBehind)
        {
            _currentPathNode++;
            //was last node so stop
            if (_currentPathNode == _path.points.Count)
            {
                _move = false;
                _currentPathNode = 0;
            }
        }
    }

    void MoveBody()
    {
        if (_currentPathNode == _path.points.Count) return;

        GetCurrentNode();

        Vector3 currentTarget = _path.points[_currentPathNode];
        Vector3 forward = transform.forward;
        Vector3 currentPos = transform.position;


        int contacts = Physics.OverlapSphereNonAlloc(currentPos, (GroundOffset  + .7f) * GetSize(), _contactPoints, _groundMask);
        if (contacts > 0)
        {
            forward = Vector3.Lerp(forward, (currentTarget - currentPos).normalized, _pathLerpTime * Time.deltaTime);

            Vector3 closestContactPoint = GetClosestContactPoint(contacts, currentPos);
            _closestContactPoint = closestContactPoint;
            Vector3 up = (currentPos - closestContactPoint).normalized;
            up = Vector3.Lerp(_lastUp, up, Time.deltaTime * _upLerpTime);
            _lastUp = up;

            Quaternion targetRotation = Quaternion.LookRotation(forward, up);
            transform.rotation = targetRotation;
        }
        _currMoveVal += MoveVarianceSpeed;
        transform.position += MoveSpeed * _varianceRange.Evaluate(Mathf.PerlinNoise1D(_currMoveVal)) * Time.deltaTime * forward;
    }

    float GetSize()
    {
        return transform.lossyScale.y;
    }

    Vector3 GetClosestContactPoint(int contacts, Vector3 pos)
    {
        float min = int.MaxValue;
        Vector3 closestCol = Vector3.zero;
        foreach (Collider col in _contactPoints.Take(contacts))
        {
            Vector3 contactPoint = col.ClosestPoint(pos);
            float d = Vector3.Distance(pos, contactPoint);

            if (d < min)
            {
                min = d;
                closestCol = contactPoint;
            }
        }

        return closestCol;
    }

    void SetUpFirstStep()
    {
        if (_startedMoving) return;
        _startedMoving = true;

        // after the first stem, make step distance lower
        void legStepSizeReset(Leg l)
        {
            l.StepDistance = StepDistance;
            _waitForFirstLegs = true;
            l.OnStep -= legStepSizeReset;
        }
        _legs[0].StepDistance = RestStepDistance;
        _legs[_legs.Length / 2].StepDistance = RestStepDistance;
        _legs[0].OnStep += legStepSizeReset;
        _legs[_legs.Length / 2].OnStep += legStepSizeReset;

    }

    void SetBodyHeight()
    {
        return;
        float groundPositionAverage = 0;

        foreach (Leg leg in _legs)
            groundPositionAverage += leg.CurrentGroundPosition.y;

        groundPositionAverage = (groundPositionAverage / _legs.Length) + GroundOffset;
        //adding wobble
        groundPositionAverage += Time.deltaTime * WobbleAmplitude * Mathf.Sin(Time.time * WobbleSpeed);

        float lerpedHeight = Lerp(_lastBodyHeight, groundPositionAverage, _heightChangeLerp * Time.deltaTime);
        transform.position = new Vector3(transform.position.x, lerpedHeight, transform.position.z);
        _lastBodyHeight = lerpedHeight;
    }

    void SetBodyHeightGeneral()
    {
        Vector3 groundPositionAverage = Vector3.zero;
        Vector3 upDir = transform.up;

        foreach (Leg leg in _legs)
            groundPositionAverage += leg.CurrentGroundPosition;

        groundPositionAverage = (groundPositionAverage / _legs.Length) + GroundOffset * upDir;
        //adding wobble
        groundPositionAverage += Time.deltaTime * WobbleAmplitude * Mathf.Sin(Time.time * WobbleSpeed) * upDir;

        //get average position
        //check distance,
        //if distance is lower than needed, move body in the plane normal
        //if can't figure out the plane normal, 

        Vector3 lerpedHeight = Vector3.Lerp(transform.position, groundPositionAverage, _heightChangeLerp * Time.deltaTime);
        transform.position = lerpedHeight;

    }


    float Lerp(float start, float end, float t)
    {
        return start + (end - start) * t;
    }

    void OnDrawGizmos()
    {
        if (_legs == null || !Gizmos) return;

        if (_currentPathNode != _path.points.Count)
        {
            UnityEngine.Gizmos.color = Color.yellow;
            Vector3 currentTarget = _path.points[_currentPathNode];

            UnityEngine.Gizmos.DrawLine(transform.position, transform.position + (currentTarget - transform.position).normalized * transform.lossyScale.y);
            UnityEngine.Gizmos.DrawSphere(currentTarget, .1f * transform.lossyScale.y);
        }


        foreach (var leg in _legs)
            leg.OnDrawGizmos();

        UnityEngine.Gizmos.color = Color.black;
        //UnityEngine.Gizmos.DrawSphere(transform.position /*+ transform.up * GetSize() * .5f*/, (GroundOffset + .7f) * GetSize());
        UnityEngine.Gizmos.DrawSphere(_closestContactPoint, .2f * transform.lossyScale.y);
    }

    void OnDisable()
    {
        foreach (var leg in _legs) leg.ClearEvents();
    }
}
