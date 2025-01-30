using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class FishingRod : MonoBehaviour
{
    public Transform bait;
    public float lerpTime = 1;
    Vector3 baitPos;

    private void Update() {
        if (TryGetClosestPlane(out Vector3 pos))
            baitPos = pos;
            
        bait.position = Vector3.Lerp(bait.position, baitPos, lerpTime * Time.deltaTime);
    }

    bool TryGetClosestPlane(out Vector3 hit)
    {
        hit = Vector3.zero;
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        List<ARRaycastHit> arRaycastHits = new List<ARRaycastHit>();

        if (arRaycastManager.Raycast(ray, arRaycastHits, UnityEngine.XR.ARSubsystems.TrackableType.Planes))
        {
            hit = arRaycastHits[0].pose.position;
            return true;
        }

        return false;
    }

    private ARRaycastManager arRaycastManager;

    void Start()
    {
        arRaycastManager = GetComponentInParent<ARRaycastManager>();
    }
}
