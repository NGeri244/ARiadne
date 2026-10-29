using System;
using System.Collections.Generic;
using UnityEngine;

public class NavigationController : MonoBehaviour
{
    const string EntranceNodeId = "n1";

    [SerializeField] MapManager map;
    [SerializeField] GraphAligner aligner;
    [SerializeField] PathVisualizer visualizer;
    [SerializeField] ChevronPool chevrons;

    readonly List<Vector3> pathBuffer = new List<Vector3>(PathVisualizer.Capacity);

    string destinationId = string.Empty;
    string lastStartId;
    string lastDestinationId;

    public string DestinationId => destinationId;

    public float RouteLengthMeters { get; private set; } = -1f;

    public event Action RouteChanged;

    void OnEnable()
    {
        if (aligner == null)
            return;

        aligner.Aligned += HandleAligned;
        aligner.AlignmentReset += HandleAlignmentReset;
    }

    void OnDisable()
    {
        if (aligner == null)
            return;

        aligner.Aligned -= HandleAligned;
        aligner.AlignmentReset -= HandleAlignmentReset;
    }

    public void SetDestination(string nodeId)
    {
        destinationId = nodeId ?? string.Empty;
        if (string.IsNullOrEmpty(destinationId))
        {
            lastStartId = null;
            lastDestinationId = null;
            HideRoute();
            return;
        }

        if (aligner != null && aligner.IsAligned)
            RequestPath();
    }

    void HandleAligned()
    {
        if (!string.IsNullOrEmpty(destinationId))
            RequestPath();
    }

    void HandleAlignmentReset()
    {
        lastStartId = null;
        lastDestinationId = null;
        HideRoute();
    }

    void RequestPath()
    {
        if (aligner == null || !aligner.IsAligned || map == null)
            return;

        if (string.IsNullOrEmpty(destinationId))
        {
            HideRoute();
            return;
        }

        string startId = ResolveStartNodeId();
        if (startId == lastStartId && destinationId == lastDestinationId)
            return;

        lastStartId = startId;
        lastDestinationId = destinationId;

        bool found = AStarPathfinder.FindPath(startId, destinationId, map.Nodes, pathBuffer);
        if (!found || pathBuffer.Count < 2)
        {
            HideRoute();
            return;
        }

        if (visualizer != null)
            visualizer.ShowPath(pathBuffer);
        if (chevrons != null)
            chevrons.Show(pathBuffer);

        float length = 0f;
        for (int i = 1; i < pathBuffer.Count; i++)
            length += Vector3.Distance(pathBuffer[i - 1], pathBuffer[i]);

        RouteLengthMeters = length;
        RouteChanged?.Invoke();
    }

    string ResolveStartNodeId()
    {
        return EntranceNodeId;
    }

    void HideRoute()
    {
        if (visualizer != null)
            visualizer.Hide();
        if (chevrons != null)
            chevrons.Hide();

        if (RouteLengthMeters < 0f)
            return;

        RouteLengthMeters = -1f;
        RouteChanged?.Invoke();
    }
}
