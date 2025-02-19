using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FishSpawner : MonoBehaviour
{
    [SerializeField] float spawnRadius = 1;
    [SerializeField] GameObject fishPrefab;

    readonly Dictionary<int, GameObject> currentFish = new();
    public Transform bait;
    int _fishId = 0;

    public void ClearFishes()
    {
        foreach (var fish in currentFish.Values)
        {
            Destroy(fish);
        }
        currentFish.Clear();
    }

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
        currentFish.Add(_fishId, newFish);
        FishMovement fish = newFish.GetComponent<FishMovement>();
        fish.fishId = _fishId;
        fish.OnDestroy += id =>
        {
            currentFish.Remove(id);
        };
        fish.OnCatch += FishingRod.Instance.CatchFish;
        _fishId++;
    }

    private bool TryGetClosestPlane(out Vector3 spawnPosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
        {
            spawnPosition = hit.point;
            return true;
        }
        spawnPosition = Vector3.zero;
        return false;
    }

    private void Start() {
        _fishId = 0;
        StartCoroutine(StartSpawning());
    }

    private void Update() {
        // Fix Fishes Y position to the plane height
        if (TryGetClosestPlane(out Vector3 hitPoint))
        {
            foreach(var fish in currentFish)
            {
                Transform fishTr = fish.Value.transform;
                fishTr.position = new(fishTr.position.x, hitPoint.y, fishTr.position.z);
            }
        }
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
}
