using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingRod : MonoBehaviour
{
    public Transform bait;
    public float lerpTime = 1;
    Vector3 baitPos;
    [SerializeField] float baitRaycastLength = 0.105f;
    
    public static FishingRod Instance { get; private set; }

    private void Awake() {
        if (Instance == null)
            Instance = this;
    }

    private void Update() {
        if (TryGetClosestGround(out Vector3 pos))
            baitPos = pos;
        Vector3 newPosition = Vector3.Lerp(bait.position, baitPos, lerpTime * Time.deltaTime);
        newPosition.y = baitPos.y;
        bait.position = newPosition;
    }

    bool TryGetClosestGround(out Vector3 hit)
    {
        hit = Vector3.zero;
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        RaycastHit raycastHit;

        if (Physics.Raycast(ray, out raycastHit, Mathf.Infinity, LayerMask.GetMask("Ground")))
        {
            hit = raycastHit.point;
            return true;
        }

        return false;
    }

    void Start()
    {
        // Initialization if needed
    }

    public bool IsBaitOnWater()
    {
        Ray ray = new Ray(bait.position + new Vector3(0, 0.1f, 0), Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, baitRaycastLength, LayerMask.GetMask("Ground")))
        {
            return true;
        }
        return false;
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(bait.position + new Vector3(0, 0.1f, 0), Vector3.down * baitRaycastLength);
    }
}
