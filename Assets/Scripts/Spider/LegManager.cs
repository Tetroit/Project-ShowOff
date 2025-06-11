using System;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class LegManager : MonoBehaviour
{
    public bool IsMoving { get; private set; }
    [SerializeField] bool _move;
    [field: SerializeField] public bool Gizmos { get; private set; } = false;

    [SerializeField] Curve _path;
    [SerializeField] LayerMask _groundMask;
    [SerializeField] float _pathLerpTime = 10;
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
    [SerializeField] AnimationCurve _varianceRange;

    [Header("Bodyheight related")]
    [Tooltip("Smoothing of changing body height")]
    [SerializeField] float _heightChangeLerp;
    [SerializeField] float _groundOffset = 1f;
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

    void Start()
    {
        _lastBodyHeight = transform.position.y;
        _legs = new Leg[]
        {
           new (AngleX, 0, firstLeg, _jointCount, this                    , ForwardReach, _groundMask),
           //new (AngleX, -AngleY, _legPrefab, _jointCount, this            , ForwardReach, _groundMask),
           new (AngleX, -AngleY*2, _legPrefab, _jointCount, this          , ForwardReach, _groundMask),

           new (AngleX, -AngleY*2 - 135, firstSecondLeg, _jointCount, this, ForwardReach, _groundMask),
           //new (AngleX, -AngleY-135, _legPrefab, _jointCount, this        , ForwardReach, _groundMask),
           new (AngleX, - 135, _legPrefab, _jointCount, this              , ForwardReach, _groundMask),

        };

        SetAdjacentLegs(_legs);
    }

    void SetAdjacentLegs( Leg[] arr)
    {
        Debug.Assert(arr.Length % 2 == 0);

        int legsOnSide = (int)(arr.Length * .5f);
        for (int i = 0; i< arr.Length; i++)//attaches left right up down neighbours but assumes that the grid has only two rows
        {
            int left = i - 1;
            int right = i+1;
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
                if (left > legsOnSide-1) adjLegs.Add(arr[left]);
                if (right < arr.Length) adjLegs.Add(arr[right]);
                adjLegs.Add(arr[up]);
            }
            
            arr[i].SetAdjacentLegs(adjLegs.ToArray());
        }
    }

    void Update()
    {
        if(_move)
        {
            SetUpFirstStep();
            if(_waitForFirstLegs)
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

    void MoveBody()
    {
        if (_currentPathNode == _path.points.Count) return;

        Vector3 currentTarget = _path.points[_currentPathNode];
        Vector3 forward = transform.forward;

        bool nodeIsBehind = Vector3.Distance(currentTarget, transform.position) < 1;
        if (nodeIsBehind)
        {
            _currentPathNode++;
            //was last node so stop
            if (_currentPathNode == _path.points.Count)
            {
                _move = false;
                _currentPathNode = 0;
                return;
            }
        }
        //move towards next node
        currentTarget = _path.points[_currentPathNode];
        forward = Vector3.Lerp(forward, (currentTarget - transform.position).normalized, _pathLerpTime * Time.deltaTime);
        _currMoveVal += MoveVarianceSpeed;
        transform.forward = forward;
        transform.position += MoveSpeed * _varianceRange.Evaluate(Mathf.PerlinNoise1D(_currMoveVal)) * Time.deltaTime * forward;
        
    }
    
    void SetUpFirstStep()
    {
        if (_startedMoving) return;
        _startedMoving = true;

        // after the first stem, make step distance lower
        void legStepSizeReset(Leg l)
        {
            l.StepSize = StepDistance;
            _waitForFirstLegs = true;
            l.OnStep -= legStepSizeReset;
        }
        _legs[0].StepSize = RestStepDistance;
        _legs[_legs.Length/2].StepSize = RestStepDistance;
        _legs[0].OnStep += legStepSizeReset;
        _legs[_legs.Length / 2].OnStep += legStepSizeReset;

    }

    void SetBodyHeight()
    {
        return;
        float groundPositionAverage = 0;

        foreach (Leg leg in _legs)
            groundPositionAverage += leg.CurrentGroundPosition.y;
        
        groundPositionAverage = (groundPositionAverage / _legs.Length) + _groundOffset;
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

        groundPositionAverage = (groundPositionAverage / _legs.Length) + _groundOffset * upDir;
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

        if( _currentPathNode != _path.points.Count)
        {
            UnityEngine.Gizmos.color = Color.yellow;
            Vector3 currentTarget = _path.points[_currentPathNode];

            UnityEngine.Gizmos.DrawLine(transform.position, transform.position + (currentTarget - transform.position).normalized);
            UnityEngine.Gizmos.DrawSphere(currentTarget, .1f);
        }


        foreach (var leg in _legs)
            leg.OnDrawGizmos();

    }

    void OnDisable()
    {
        foreach (var leg in _legs) leg.ClearEvents();
    }
}
