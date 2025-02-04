using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FishSpawner : MonoBehaviour
{
    [SerializeField] float spawnRadius = 1;
    [SerializeField] float spawnTime = 0.5f;
    [SerializeField] float catchTime = 0.5f;
    [SerializeField] GameObject fishPrefab;
    Dictionary<int, GameObject> currentFish = new();
    public Transform catchAlert;
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
        fish.bait = bait;
        fish.fishId = _fishId;
        fish.OnCatch += OnBait;
        StartCoroutine(FishSpawnAnimation(newFish.GetComponentInChildren<SpriteRenderer>(), spawn:true));
        _fishId++;
    }

    private void OnBait(int fishId)
    {
        catchAlert.gameObject.SetActive(true);
        StartCoroutine(CatchTimer(fishId));
    }

    IEnumerator CatchTimer(int fishId)
    {
        float timer = 0;
        while(timer < catchTime)
        {
            if (!FishingRod.Instance.IsBaitOnWater())
            {
                print("Yeah! I caught a fish");
                Destroy(currentFish[fishId]);
                currentFish.Remove(fishId);
                catchAlert.gameObject.SetActive(false);
                FindFirstObjectByType<FishDialogs>().GetRandomFish();
                yield break;
            }
            yield return null;
            timer += Time.deltaTime;
        }
        StartCoroutine(FishSpawnAnimation(currentFish[fishId].GetComponentInChildren<SpriteRenderer>(), false));
        Destroy(currentFish[fishId], spawnTime + 0.1f);
        currentFish.Remove(fishId);
        catchAlert.gameObject.SetActive(false);
        FindFirstObjectByType<FishDialogs>().LostCatch();
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
        catchAlert.gameObject.SetActive(false);
    }

    private void Update() {
        // print("IsBait on water: " + IsBaitOnWater());
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

    IEnumerator FishSpawnAnimation(SpriteRenderer newFishRenderer, bool spawn)
    {
        // Fade the fish from transparent to opaque
        float time = 0;
        Color color = newFishRenderer.color;
        while (time < spawnTime)
        {
            color.a = time / spawnTime;
            if (!spawn)
                color.a = 1 - color.a;
            newFishRenderer.color = color;
            time += Time.deltaTime;
            yield return null;
        }
        color.a = spawn? 1: 0;
        newFishRenderer.color = color;
    }
}
