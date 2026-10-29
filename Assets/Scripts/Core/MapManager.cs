using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    readonly Dictionary<string, NavigationNode> nodes = new Dictionary<string, NavigationNode>(8);
    readonly List<NavigationNode> destinations = new List<NavigationNode>(4);

    public IReadOnlyDictionary<string, NavigationNode> Nodes => nodes;

    public IReadOnlyList<NavigationNode> Destinations => destinations;

    void Awake()
    {
        CreateTestNetwork();
    }

    void CreateTestNetwork()
    {
        var entrance = new NavigationNode("n1", "Entrance", new Vector3(0f, 0f, 0f), true);
        var hallway = new NavigationNode("n2", "Hallway", new Vector3(0f, 0f, 6.5f), false);
        var room204 = new NavigationNode("n3", "Room 204", new Vector3(3.2f, 0f, 9.8f), true);

        AddEdge(entrance, hallway);
        AddEdge(hallway, room204);

        nodes.Add(entrance.id, entrance);
        nodes.Add(hallway.id, hallway);
        nodes.Add(room204.id, room204);

        destinations.Clear();
        if (entrance.isDestination)
            destinations.Add(entrance);
        if (hallway.isDestination)
            destinations.Add(hallway);
        if (room204.isDestination)
            destinations.Add(room204);
    }

    static void AddEdge(NavigationNode a, NavigationNode b)
    {
        float weight = Vector3.Distance(a.position, b.position);
        a.AddEdge(b.id, weight);
        b.AddEdge(a.id, weight);
    }
}
