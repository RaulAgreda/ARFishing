using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour
{
    public float shipVelocity = 0.05f;
    public float pointVelocity = 30f;
    public float fireCooldown = 2f;
    public Transform yawCannonAxis;
    public Transform pitchCannonAxis;
    public GameObject projectilePrefab;
    public float projectileSpeed = 0.2f;
    Vector3 targetMovePosition;
    float lastTimeFired = 0f;

    enum ShipState {Moving, Pointing, Firing};
    ShipState state = ShipState.Moving;

    public void Move(Vector3 target)
    {
        transform.LookAt(target);
        targetMovePosition = target;
    }

    public void PointAtCamera()
    {
        Vector3 target = Camera.main.transform.position;
        target.y = yawCannonAxis.position.y;
        yawCannonAxis.LookAt(target);

        // target = pitchCannonAxis.forward + pitchCannonAxis.position;
        // target.y = Camera.main.transform.position.y;
        pitchCannonAxis.LookAt(Camera.main.transform.position);
    }

    IEnumerator PointAtCameraCoroutine()
    {
        while (true)
        {
            Vector3 target = Camera.main.transform.position;
            target.y = yawCannonAxis.position.y;
            yawCannonAxis.rotation = Quaternion.RotateTowards(yawCannonAxis.rotation, Quaternion.LookRotation(target - yawCannonAxis.position), pointVelocity * Time.deltaTime);

            if (Vector3.Angle(yawCannonAxis.forward, target - yawCannonAxis.position) < 1f)
            {
                Quaternion pitchRotation = Quaternion.LookRotation(Camera.main.transform.position - pitchCannonAxis.position);
                pitchCannonAxis.rotation = Quaternion.RotateTowards(pitchCannonAxis.rotation, pitchRotation, pointVelocity * Time.deltaTime);
            }
            if (PointingAtTarget())
            {
                state = ShipState.Firing;
                break;
            }
            yield return null;
        }
    }

    bool PointingAtTarget()
    {
        float angle = Vector3.Angle(pitchCannonAxis.forward, Camera.main.transform.position - pitchCannonAxis.position);
        Debug.Log("Angle to target: " + angle);
        return angle < 1f;
    }

    void Fire()
    {
        GameObject projectile = Instantiate(projectilePrefab, pitchCannonAxis.position, pitchCannonAxis.rotation);
        projectile.GetComponent<Rigidbody>().velocity = pitchCannonAxis.forward * projectileSpeed;
        lastTimeFired = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (state == ShipState.Moving)
        {
            transform.Translate(shipVelocity * Time.deltaTime * Vector3.forward);
            if (Vector3.Distance(transform.position, targetMovePosition) < 0.01f)
            {
                StartCoroutine(PointAtCameraCoroutine());
                state = ShipState.Pointing;
            }
        }
        else if (state == ShipState.Firing)
        {
            PointAtCamera();
            if (Time.time - lastTimeFired > fireCooldown)
                Fire();
        }

        // PointAtCamera();
    }
}
