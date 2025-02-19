using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenpeaceSpawner : MonoBehaviour
{
    public int numberOfShips = 3;
    public GameObject shipPrefab;
    public float spawnDistance = 0.1f;
    Vector3 RandomScreenPoint(Vector3 groundPoint)
    {
        Vector3 randomScreenPoint = new(
            Random.Range(0, Screen.width),
            Random.Range(0, Screen.height),
            Vector3.Distance(Camera.main.transform.position, groundPoint)
        );
        Vector3 randomWorldPoint = Camera.main.ScreenToWorldPoint(randomScreenPoint);
        randomWorldPoint.y = groundPoint.y;
        return randomWorldPoint;
    }

    Vector3 RandomPointOutsideScreen(Vector3 groundPoint, float distance)
    {
        Vector3 randomScreenPoint;
        if (Random.value > 0.5f)
        {
            randomScreenPoint = new Vector3(
                Random.value > 0.5f ? Random.Range(-distance, distance) : Random.Range(Screen.width - distance, Screen.width + distance),
                Random.Range(0, Screen.height),
                Vector3.Distance(Camera.main.transform.position, groundPoint)
            );
        }
        else
        {
            randomScreenPoint = new Vector3(
                Random.Range(0, Screen.width),
                Random.value > 0.5f ? Random.Range(-distance, distance) : Random.Range(Screen.height - distance, Screen.height + distance),
                Vector3.Distance(Camera.main.transform.position, groundPoint)
            );
        }

        Vector3 randomWorldPoint = Camera.main.ScreenToWorldPoint(randomScreenPoint);
        randomWorldPoint.y = groundPoint.y;
        randomWorldPoint += (randomWorldPoint - groundPoint).normalized * distance;
        return randomWorldPoint;
    }

    [SerializeField]
    bool canSpawn = false;
    bool haveSpawned = false;

    void Update()
    {

        if (!haveSpawned && canSpawn)
        {
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit))
            {
                for (int i = 0; i < numberOfShips; i++)
                {
                    GameObject ship = Instantiate(shipPrefab, RandomPointOutsideScreen(hit.point, spawnDistance), Quaternion.identity);
                    
                }
                haveSpawned = true;
            }
        }
        
    }

}
