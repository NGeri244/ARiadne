using System;

[Serializable]
public class NavigationEdge
{
    public string targetNodeId;
    public float weight;

    public NavigationEdge(string targetNodeId, float weight)
    {
        this.targetNodeId = targetNodeId;
        this.weight = weight;
    }
}
