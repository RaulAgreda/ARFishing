using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class FishMovement : MonoBehaviour
{
    enum FishState { Idle, RandomMove, FollowingTarget, lookToBait, movingToBait, bittenBait };

    FishState currentFishState;
    [SerializeField] float maxSpeed = 1;
    [SerializeField] float minSpeed = 0.1f;
    [SerializeField] float randomRadius = 0.1f;
    [SerializeField] float rotationLerp = 0.5f;
    [SerializeField] float detectionAngle = 15f;
    [SerializeField] float maxDetectionDistance = 0.1f;

    public Transform bait;
    private Vector3 currentTargetPos;
    private float currentSpeed;

    ARPlaneManager planeManager;

    private void Start() {
        planeManager = FindFirstObjectByType<ARPlaneManager>();
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
        return angle <= detectionAngle / 2 && distance < maxDetectionDistance;
    }

    float AngleTo(Vector3 position)
    {
        position.y = transform.position.y;
        return Vector3.Angle(position - transform.position, transform.forward);
    }

    float DistanceTo(Vector3 position)
    {
        position.y = transform.position.y;
        return Vector3.Distance(position, transform.position);
    }

    void GoBack()
    {
        float randomDistance = Random.Range(minSpeed, maxSpeed);
        transform.Translate(-randomDistance * Time.deltaTime * Vector3.forward);
    }

    private void Update() {
        if (currentFishState == FishState.Idle)
        {
            currentTargetPos = transform.position + 
            new Vector3(Random.Range(-1, 1f), 0, Random.Range(-1,1f)).normalized * randomRadius;
            currentFishState = FishState.RandomMove;
            currentSpeed = Random.Range(minSpeed, maxSpeed);
        }
        else if (currentFishState == FishState.RandomMove)
        {
            LookTarget(currentTargetPos);
            GoForward();
            if (DistanceTo(currentTargetPos) < 0.001f)
            {
                currentFishState = FishState.Idle;
            }
            if (DetectBait())
            {
                currentFishState = FishState.lookToBait;
            }
        }
        else if (currentFishState == FishState.lookToBait)
        {
            float angle = AngleTo(bait.position);
            // print("Angle is: " + angle);
            LookTarget(bait.position);
            if (angle < 1f)
                currentFishState = FishState.movingToBait;
        }
        else if (currentFishState == FishState.movingToBait)
        {
            GoForward();
            if (!DetectBait())
            {
                currentFishState = FishState.Idle;
            }
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
