using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class PathVisualizer : MonoBehaviour
{
    public const int Capacity = 32;
    const float FloorLiftMeters = 0.015f;

    [SerializeField] LineRenderer line;
    [SerializeField] Transform graphSpace;
    [SerializeField] GraphAligner aligner;

    NativeArray<Vector3> positions;

    void Awake()
    {
        if (line == null)
            line = GetComponent<LineRenderer>();

        positions = new NativeArray<Vector3>(Capacity, Allocator.Persistent, NativeArrayOptions.ClearMemory);
        if (line != null)
            line.positionCount = 0;
    }

    void OnDestroy()
    {
        if (positions.IsCreated)
            positions.Dispose();
    }

    public void ShowPath(IReadOnlyList<Vector3> localPoints)
    {
        if (line == null || graphSpace == null || aligner == null || !aligner.IsAligned
            || localPoints == null || localPoints.Count < 2)
        {
            Hide();
            return;
        }

        int count = localPoints.Count;
        if (count > positions.Length)
            count = positions.Length;

        for (int i = 0; i < count; i++)
        {
            Vector3 world = graphSpace.TransformPoint(localPoints[i]);
            world.y += FloorLiftMeters;
            positions[i] = world;
        }

        line.positionCount = count;
        line.SetPositions(positions.GetSubArray(0, count));
        line.enabled = true;
    }

    public void Hide()
    {
        if (line == null)
            return;

        line.positionCount = 0;
        line.enabled = false;
    }
}
