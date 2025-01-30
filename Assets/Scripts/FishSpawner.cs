using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARFoundation;

public class FishSpawner : MonoBehaviour
{
    [SerializeField] float spawnRadius = 1;
    [SerializeField] float spawnTime = 0.5f;
    [SerializeField] ARRaycastManager raycastManager;
    [SerializeField] GameObject fishPrefab;
    List<GameObject> currentFish = new();
    public Transform bait;

    public void Spawn(Vector3 spawnPosition)
    {
        if (currentFish.Count > 5)
            return;
        Debug.Log("Spawning fish");
        Debug.Log("Plane found, spawning fish");
        Vector3 randomDir = new Vector3(Random.Range(-1, 1f), 0, Random.Range(-1, 1f)).normalized * spawnRadius;
        Vector3 finalPosition = spawnPosition + randomDir;
        GameObject newFish = Instantiate(fishPrefab, finalPosition,
        Quaternion.Euler(0, Random.Range(0, 360), 0));
        currentFish.Add(newFish);
        newFish.GetComponent<FishMovement>().bait = bait;
        StartCoroutine(FishSpawnAnimation(newFish.GetComponentInChildren<SpriteRenderer>()));
    }

    private bool TryGetClosestPlane(out Vector3 spawnPosition)
    {
        List<ARRaycastHit> hits = new();
        Vector3 screenCenter = new(Screen.width / 2, Screen.height / 2);
        if (raycastManager.Raycast(screenCenter, hits, TrackableType.PlaneWithinBounds))
        {
            foreach (var hit in hits)
            {
                if (hit.trackable is ARPlane plane && plane.alignment == PlaneAlignment.HorizontalUp)
                {
                    spawnPosition = hit.pose.position;
                    return true;
                }
            }
        }
        spawnPosition = Vector3.zero;
        return false;
    }

    private void Start() {
        StartCoroutine(StartSpawning());   
    }

    private IEnumerator StartSpawning()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            if (TryGetClosestPlane(out Vector3 spawnPosition))
            {
                Spawn(spawnPosition);
                // yield break;
            }
        }
    }

    IEnumerator FishSpawnAnimation(SpriteRenderer newFishRenderer)
    {
        // Fade the fish from transparent to opaque
        float time = 0;
        Color color = newFishRenderer.color;
        while (time < spawnTime)
        {
            color.a = time / spawnTime;
            newFishRenderer.color = color;
            time += Time.deltaTime;
            yield return null;
        }
        color.a = 1;
        newFishRenderer.color = color;
    }
}
