using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineSpawner : MonoBehaviour
{
    public int mineCount = 8;
    public GameObject minePrefab;
    int _currentMines = 0;
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
    
    IEnumerator SpawnMinesLoop()
    {
        while (true)
        {
            if (_currentMines < mineCount && RandomScreenPoint(out Vector3 groundPoint))
            {
                Instantiate(minePrefab, groundPoint, Quaternion.identity);
                _currentMines++;
            }
            yield return new WaitForSeconds(1);
        }
    }
}
