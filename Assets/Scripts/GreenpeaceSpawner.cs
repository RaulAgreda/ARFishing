using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenpeaceSpawner : MonoBehaviour
{
    public float marginAngle = 5f;
    public GameObject shipPrefab;
    public float distanceFromCenter = 0.2f;

    [SerializeField]
    bool canSpawn = false;
    bool haveSpawned = false;

    public void StartSpawning()
    {
        canSpawn = true;
    }

    void Update()
    {
        if (!haveSpawned && canSpawn)
        {
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit))
            {
                List<ShipController> ships = new List<ShipController>();
                // First point at 0.8 screen height at the left of the screen
                Vector3 leftPoint = CalculateSpawnPosition(new (0, 0.8f), hit.distance);
                leftPoint.y = hit.point.y;
                ships.Add(Instantiate(shipPrefab, leftPoint, Quaternion.identity).GetComponent<ShipController>());
                // Second point at 0.8 screen height at the right of the screen
                Vector3 rightPoint = CalculateSpawnPosition(new (1, 0.7f), hit.distance);
                print("Right point: " + rightPoint);
                rightPoint.y = hit.point.y;
                ships.Add(Instantiate(shipPrefab, rightPoint, Quaternion.identity).GetComponent<ShipController>());
                // Third point at the middle bottom of the screen
                Vector3 bottomPoint = CalculateSpawnPosition(new (0.4f, 0), hit.distance);
                print("Bottom point: " + bottomPoint);
                bottomPoint.y = hit.point.y;
                ships.Add(Instantiate(shipPrefab, bottomPoint, Quaternion.identity).GetComponent<ShipController>());

                foreach (ShipController ship in ships)
                {
                    Vector3 direction = (ship.transform.position - hit.point).normalized * Random.Range(distanceFromCenter, distanceFromCenter + 0.05f);
                    Vector3 movePoint = hit.point + direction;
                    ship.Move(movePoint);
                }
            }
            haveSpawned = true;
        }
        
    }

    Vector3 CalculateSpawnPosition(Vector2 viewportPos, float spawnDistance)
    {
        Camera mainCamera = Camera.main;
        // Verifica que la posición esté en el borde
        bool isEdge = viewportPos.x == 0 || viewportPos.x == 1 || viewportPos.y == 0 || viewportPos.y == 1;
        if (!isEdge)
        {
            Debug.LogError("¡La posición no está en el borde de la pantalla!");
            return Vector3.zero;
        }

        float vFOV = mainCamera.fieldOfView;
        float hFOV = 2 * Mathf.Atan(Mathf.Tan(vFOV * 0.5f * Mathf.Deg2Rad) * mainCamera.aspect) * Mathf.Rad2Deg;

        Vector3 directionOffset = Vector3.zero;

        // Desplazamiento horizontal (bordes izquierdo/derecho)
        if (viewportPos.x == 0 || viewportPos.x == 1)
        {
            bool isRight = viewportPos.x == 1;
            float angleX = (hFOV / 2 + marginAngle) * (isRight ? 1 : -1);
            float offsetX = Mathf.Tan(angleX * Mathf.Deg2Rad) * spawnDistance;
            directionOffset += mainCamera.transform.right * offsetX;

            // Desplazamiento vertical proporcional (ej: esquinas)
            float yPos = (viewportPos.y - 0.5f) * 2; // Convierte a rango [-1, 1]
            float angleY = yPos * (vFOV / 2);
            float offsetY = Mathf.Tan(angleY * Mathf.Deg2Rad) * spawnDistance;
            directionOffset += mainCamera.transform.up * offsetY;
        }

        // Desplazamiento vertical (bordes superior/inferior)
        if (viewportPos.y == 0 || viewportPos.y == 1)
        {
            bool isTop = viewportPos.y == 1;
            float angleY = (vFOV / 2 + marginAngle) * (isTop ? 1 : -1);
            float offsetY = Mathf.Tan(angleY * Mathf.Deg2Rad) * spawnDistance;
            directionOffset += mainCamera.transform.up * offsetY;

            // Desplazamiento horizontal proporcional (ej: esquinas)
            float xPos = (viewportPos.x - 0.5f) * 2; // Convierte a rango [-1, 1]
            float angleX = xPos * (hFOV / 2);
            float offsetX = Mathf.Tan(angleX * Mathf.Deg2Rad) * spawnDistance;
            directionOffset += mainCamera.transform.right * offsetX;
        }

        // Posición final
        Vector3 spawnDirection = mainCamera.transform.forward * spawnDistance + directionOffset;
        return mainCamera.transform.position + spawnDirection;
    }

}
