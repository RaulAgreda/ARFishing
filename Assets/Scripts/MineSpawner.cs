using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineSpawner : MonoBehaviour
{
    public int mineCount = 8;
    public GameObject minePrefab;
    int _currentMines = 0;
    
    // Dictionary to track spawned mines
    private readonly Dictionary<int, GameObject> spawnedMines = new Dictionary<int, GameObject>();
    private int _mineId = 0;
    
    // Helper method to detect ground plane
    private bool TryGetClosestPlane(out Vector3 hitPoint)
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
        {
            hitPoint = hit.point;
            return true;
        }
        hitPoint = Vector3.zero;
        return false;
    }
    
    bool RandomScreenPoint(out Vector3 groundPoint)
    {
        Vector3 randomScreenPoint = new(
            Random.Range(0, Screen.width),
            Random.Range(0, Screen.height),
            0
        );
        Ray ray = Camera.main.ScreenPointToRay(randomScreenPoint);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
        {
            groundPoint = hit.point;
            return true;
        }
        groundPoint = Vector3.zero;
        return false;
    }

    public void SpawnMines()
    {
        StartCoroutine(SpawnMinesLoop());
    }

    public void MineDestroyed()
    {
        _currentMines--;
    }
    
    void Update()
    {
        // Update mines Y position to match ground height
        if (TryGetClosestPlane(out Vector3 hitPoint))
        {
            foreach (var mine in spawnedMines.Values)
            {
                if (mine != null)
                {
                    Vector3 position = mine.transform.position;
                    mine.transform.position = new Vector3(position.x, hitPoint.y, position.z);
                }
            }
        }
    }
    
    IEnumerator SpawnMinesLoop()
    {
        while (true)
        {
            if (_currentMines < mineCount && RandomScreenPoint(out Vector3 groundPoint))
            {
                GameObject newMine = Instantiate(minePrefab, groundPoint, Quaternion.identity);
                spawnedMines.Add(_mineId++, newMine);
                _currentMines++;
            }
            yield return new WaitForSeconds(1);
        }
    }
}
