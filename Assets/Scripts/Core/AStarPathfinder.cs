using System.Collections.Generic;
using UnityEngine;

public static class AStarPathfinder
{
    private sealed class PathNode
    {
        public NavigationNode Node;
        public PathNode Parent;
        public float G;
        public float H;
        public bool IsOpen;

        public float F => G + H;

        public void Reset(NavigationNode node, PathNode parent, float g, float h, bool isOpen)
        {
            Node = node;
            Parent = parent;
            G = g;
            H = h;
            IsOpen = isOpen;
        }
    }

    private static readonly List<PathNode> Open = new List<PathNode>(32);
    private static readonly Dictionary<string, PathNode> Visited = new Dictionary<string, PathNode>(32);
    private static readonly Stack<PathNode> Pool = new Stack<PathNode>(32);

    public static bool FindPath(
        string startNodeId,
        string targetNodeId,
        IReadOnlyDictionary<string, NavigationNode> graph,
        List<Vector3> results)
    {
        if (results == null)
            return false;

        results.Clear();
        ReleaseScratch();

        if (graph == null || string.IsNullOrEmpty(startNodeId) || string.IsNullOrEmpty(targetNodeId))
            return false;

        if (!graph.TryGetValue(startNodeId, out NavigationNode start) || start == null)
            return false;

        if (!graph.TryGetValue(targetNodeId, out NavigationNode target) || target == null)
            return false;

        PathNode startRecord = Rent();
        startRecord.Reset(start, null, 0f, Vector3.Distance(start.position, target.position), true);
        Visited[start.id] = startRecord;
        Open.Add(startRecord);

        while (Open.Count > 0)
        {
            int bestIndex = FindLowestF();
            PathNode current = Open[bestIndex];
            int last = Open.Count - 1;
            Open[bestIndex] = Open[last];
            Open.RemoveAt(last);
            current.IsOpen = false;

            if (current.Node.id == targetNodeId)
                return BuildPath(current, results);

            List<NavigationEdge> edges = current.Node.edges;
            if (edges == null)
                continue;

            for (int i = 0; i < edges.Count; i++)
            {
                NavigationEdge edge = edges[i];
                if (edge == null || string.IsNullOrEmpty(edge.targetNodeId))
                    continue;

                if (!graph.TryGetValue(edge.targetNodeId, out NavigationNode neighbor) || neighbor == null)
                    continue;

                float tentativeG = current.G + edge.weight;
                if (Visited.TryGetValue(neighbor.id, out PathNode known))
                {
                    if (tentativeG >= known.G)
                        continue;

                    known.G = tentativeG;
                    known.H = Vector3.Distance(neighbor.position, target.position);
                    known.Parent = current;

                    if (!known.IsOpen)
                    {
                        known.IsOpen = true;
                        Open.Add(known);
                    }

                    continue;
                }

                PathNode next = Rent();
                next.Reset(neighbor, current, tentativeG, Vector3.Distance(neighbor.position, target.position), true);
                Visited[neighbor.id] = next;
                Open.Add(next);
            }
        }

        return false;
    }

    private static int FindLowestF()
    {
        int bestIndex = 0;
        float bestF = Open[0].F;
        float bestH = Open[0].H;

        for (int i = 1; i < Open.Count; i++)
        {
            PathNode candidate = Open[i];
            float f = candidate.F;
            if (f < bestF || (f == bestF && candidate.H < bestH))
            {
                bestIndex = i;
                bestF = f;
                bestH = candidate.H;
            }
        }

        return bestIndex;
    }

    private static bool BuildPath(PathNode end, List<Vector3> results)
    {
        int count = 0;
        for (PathNode step = end; step != null; step = step.Parent)
            count++;

        if (results.Capacity < count)
            results.Capacity = count;

        for (int i = 0; i < count; i++)
            results.Add(Vector3.zero);

        for (PathNode step = end; step != null; step = step.Parent)
        {
            count--;
            results[count] = step.Node.position;
        }

        return true;
    }

    private static PathNode Rent()
    {
        if (Pool.Count > 0)
            return Pool.Pop();

        return new PathNode();
    }

    private static void ReleaseScratch()
    {
        Open.Clear();

        foreach (KeyValuePair<string, PathNode> pair in Visited)
        {
            PathNode record = pair.Value;
            record.Parent = null;
            record.Node = null;
            record.IsOpen = false;
            Pool.Push(record);
        }

        Visited.Clear();
    }
}
