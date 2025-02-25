using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehavior : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Camera>() != null)
        {
            FindFirstObjectByType<PlayerHealth>().TakeDamage();
            Destroy(gameObject);
        }
    }
}
