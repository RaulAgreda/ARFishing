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
    [SerializeField] float spawnTime = 1f;
    [SerializeField] float fishLifeTime = 30f;
    [SerializeField] AudioClip baitBittenAudio;
    public int fishId;
    private Transform bait;
    private Vector3 currentTargetPos;
    private float currentSpeed;
    Animator anim;

    float _backDistance = 0;

    public UnityAction<FishMovement> OnCatch;
    public UnityAction<int> OnDestroy;

    private void Start() {
        bait = GameObject.FindGameObjectWithTag("Bait").transform;
        anim = GetComponent<Animator>();
        currentLifeTime = 0;
        StartCoroutine(FishSpawnAnimation(true));
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

        return FishingRod.Instance.fishingState == FishingRod.FishingState.None 
        && angle <= detectionAngle / 2 && distance < maxDetectionDistance 
        && FishingRod.Instance.IsBaitOnWater();
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
            // Despawn the fish if it is too old
            if (currentLifeTime > fishLifeTime)
                DespawnFish();
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
                    OnCatch.Invoke(this);
                    anim.SetTrigger("Bite");
                    print("Bitten!");
                }
                else
                {
                    GoBack();
                    AudioManager.instance.PlayClip(baitBittenAudio, false);
                    bait.GetComponent<Animator>().SetTrigger("Bite");
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

    IEnumerator FishSpawnAnimation(bool spawn)
    {
        // Fade the fish from transparent to opaque
        float time = 0;
        var fishRenderer = GetComponentInChildren<SpriteRenderer>();
        Color color = fishRenderer.color;
        while (time < spawnTime)
        {
            color.a = time / spawnTime;
            if (!spawn)
                color.a = 1 - color.a;
            fishRenderer.color = color;
            time += Time.deltaTime;
            yield return null;
        }
        color.a = spawn? 1: 0;
        fishRenderer.color = color;
    }

    public void DespawnFish()
    {
        StartCoroutine(FishSpawnAnimation(false));
        OnDestroy.Invoke(fishId);
        Destroy(gameObject, spawnTime + 0.1f);
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