using UnityEngine;

public class FishingRodRope : MonoBehaviour
{
    public int chainLength = 10;
    public float nodeLength = 0.01f;
    public float gravity = -9.81f;
    private Vector3[] chainNodes;
    private Vector3[] prevChainNodes;
    public Transform fishingRodTip;
    public Transform bait;

    LineRenderer lineR;

    private void Awake()
    {
        lineR = GetComponent<LineRenderer>();
        lineR.positionCount = chainLength;
        chainNodes = new Vector3[chainLength];
        prevChainNodes = new Vector3[chainLength];
        for (int i = 0; i < chainLength; i++)
        {
            chainNodes[i] = fishingRodTip.position;
            prevChainNodes[i] = fishingRodTip.position;
        }
    }

    private void Update()
    {
        SimulateRope();
        lineR.SetPositions(chainNodes);
    }

    void SimulateRope()
    {
        // Apply gravity to the bait
        Vector3 moveVelocity = new Vector3(0, gravity * Time.deltaTime, 0);
        for (int i = 1; i < chainLength; i++)
            chainNodes[i] += moveVelocity;

        // Constrain the rope
        chainNodes[0] = fishingRodTip.position;
        for (int i = 1; i < chainLength; i++)
        {
            Vector3 dir = chainNodes[i] - chainNodes[i - 1];
            float dist = dir.magnitude;
            if (dist > nodeLength)
            {
                dir = dir.normalized * nodeLength;
                chainNodes[i] = chainNodes[i - 1] + dir;
            }
        }
        // Raycast from the tip to the floor
        if (Physics.Raycast(fishingRodTip.position, Vector3.down, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
            // If the rope hits the ground, stop the rope at the ground level
            if (chainNodes[^1].y < hit.point.y)
                chainNodes[^1] = new Vector3(chainNodes[^1].x, hit.point.y, chainNodes[^1].z);
        // Update bait position
        bait.position = chainNodes[^1];
        // Ensure the y position is not lower than the next node
        for (int i = 1; i < chainLength - 1; i++)
            if (chainNodes[i].y < chainNodes[^1].y)
                chainNodes[i].y = chainNodes[^1].y;
    }
}