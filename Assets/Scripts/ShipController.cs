using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour
{
    public float shipVelocity = 0.05f;
    Vector3 targetMovePosition;
    public void Move(Vector3 target)
    {
        transform.LookAt(target);
        targetMovePosition = target;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, targetMovePosition) > 0.01f)
            transform.Translate(shipVelocity * Time.deltaTime * Vector3.forward);
    }
}
