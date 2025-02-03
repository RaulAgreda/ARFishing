using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    public Vector3 offset;
    void Update()
    {
        transform.LookAt(Camera.main.transform);
        transform.localPosition = offset;
    }
}
