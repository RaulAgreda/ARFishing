using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BaitEventManager : MonoBehaviour
{
    public UnityAction<Collider> OnBaitEaten;
    void OnTriggerEnter(Collider other)
    {
        OnBaitEaten?.Invoke(other);
    }
}
