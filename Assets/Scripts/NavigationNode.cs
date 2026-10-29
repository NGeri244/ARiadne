using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class NavigationNode
{
    public string id;
    public string name;
    public Vector3 position;
    public bool isDestination;
    public List<NavigationEdge> edges = new List<NavigationEdge>();

    public NavigationNode(string id, string name, Vector3 position, bool isDestination = false)
    {
        this.id = id;
        this.name = name;
        this.position = position;
        this.isDestination = isDestination;
    }

    public void AddEdge(string targetNodeId, float weight)
    {
        edges.Add(new NavigationEdge(targetNodeId, weight));
    }
}
