using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.ARFoundation;

public class FishMovement : MonoBehaviour
{
    public enum FishState { Idle, RandomMove, FollowingTarget, lookToBait, movingToBait, movingBack, bittenBait };

    public FishState CurrentFishState { get => _currentFishState; }
    public float currentLifeTime = 0;
    FishState _currentFishState;
    [SerializeField] float maxSpeed = 1;
    [SerializeField] float minSpeed = 0.1f;
    [SerializeField] float movingToBaitSpeed = 0.1f;
    [SerializeField] float randomRadius = 0.1f;
    [SerializeField] float rotationLerp = 0.5f;
    [SerializeField] float detectionAngle = 15f;
    [SerializeField] float maxDetectionDistance = 0.1f;
    [SerializeField] float changeMovementProbability = 0.01f;
    public int fishId;
    public Transform bait;
    private Vector3 currentTargetPos;
    private float currentSpeed;
    Animator anim;

    ARPlaneManager planeManager;
    float _backDistance = 0;

    public UnityAction<int> OnCatch;

    private void Start() {
        anim = GetComponent<Animator>();
        planeManager = FindFirstObjectByType<ARPlaneManager>();
        currentLifeTime = 0;
        StartCoroutine(RandomDirection());
    }

    void LookTarget(Vector3 targetPosition)
    {
        Vector3 lookPosition = targetPosition;
        lookPosition.y = transform.position.y;
        Quaternion lookRot = Quaternion.LookRotation(lookPosition - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, rotationLerp* Time.deltaTime);
    }

    void GoForward()
    {
        transform.Translate(currentSpeed * Time.deltaTime * Vector3.forward);
    }

    public bool DetectBait()
    {
        float angle = AngleTo(bait.position);
        float distance = DistanceTo(bait.position);
        return angle <= detectionAngle / 2 && distance < maxDetectionDistance && FishingRod.Instance.IsBaitOnWater();
    }

    float AngleTo(Vector3 position)
    {
        Vector3 pos = position;
        pos.y = transform.position.y;
        return Vector3.Angle(pos - transform.position, transform.forward);
    }

    float DistanceTo(Vector3 position)
    {
        Vector3 pos = position;
        pos.y = transform.position.y;
        return Vector3.Distance(position, transform.position);
    }

    void GoBack()
    {
        _backDistance = Random.Range(minSpeed, maxSpeed) * 2;
        transform.Translate(_backDistance * Time.deltaTime * Vector3.back);
    }

    private void Update() {
        if (_currentFishState == FishState.Idle)
        {
            currentTargetPos = transform.position + 
            new Vector3(Random.Range(-1, 1f), 0, Random.Range(-1,1f)).normalized * randomRadius;
            _currentFishState = FishState.RandomMove;
            currentSpeed = Random.Range(minSpeed, maxSpeed);
        }
        else if (_currentFishState == FishState.RandomMove)
        {
            LookTarget(currentTargetPos);
            GoForward();
            if (DistanceTo(currentTargetPos) < 0.001f)
            {
                _currentFishState = FishState.Idle;
            }
            if (DetectBait())
            {
                _currentFishState = FishState.lookToBait;
            }
        }
        else if (_currentFishState == FishState.lookToBait)
        {
            float angle = AngleTo(bait.position);
            // print("Angle is: " + angle);
            LookTarget(bait.position);
            if (angle < 1f)
                _currentFishState = FishState.movingToBait;
        }
        else if (_currentFishState == FishState.movingToBait)
        {
            currentSpeed = movingToBaitSpeed;
            LookTarget(bait.position);
            GoForward();
            if (!DetectBait())
            {
                _currentFishState = FishState.Idle;
            }
            if (DistanceTo(bait.position) < 0.01f)
            {
                if (Random.value < 0.25f)
                {
                    _currentFishState = FishState.bittenBait;
                    OnCatch.Invoke(fishId);
                    anim.SetTrigger("Bite");
                    print("Bitten!");
                }
                else
                {
                    GoBack();
                    _currentFishState = FishState.movingBack;
                }
            }
        }
        else if (_currentFishState == FishState.movingBack)
        {
            if (DistanceTo(bait.position) <= _backDistance)
                GoBack();
            else
                _currentFishState = FishState.movingToBait;
        }

        currentLifeTime += Time.deltaTime;
    }
    
    IEnumerator RandomDirection()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            if (_currentFishState == FishState.RandomMove && Random.value < changeMovementProbability)
                _currentFishState = FishState.Idle;
        }
    }

    private void OnDrawGizmos() {
        // Draw current target pos
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(currentTargetPos, 0.01f);
        
        // Draw detection ray
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * maxDetectionDistance);
    }
}
