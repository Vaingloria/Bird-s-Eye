using UnityEngine;
using System.Collections.Generic;

public class A_Star : MonoBehaviour
{
    public static A_Star instance;

    private void Awake()
    {
        instance = this;
    }

/*
    public List<Node> GeneratePath(Node start, Node end) //commented this out because it was breaking everything
    {
        List<Node> openSet = new List<Node>();

        foreach(Node n in FindObjectsOfType<Node>())
        {
            n.gScore = float.MaxValue;
        }

        start.gScore = 0;
        start.hScore = Vector2.Distance(start.transform.position, end.transform.position);
        openSet.Add(start);

        while(openSet.count > 0)
        {
            int lowestF = default;

            for(int i = 1; i < openSet.Count; i++)
            {
                if(openSet[i].FScore() < openSet[lowestF].FScore())
                {
                    lowestF = i;
                }
            }

            Node currentNode = openSet[lowestF];
            openSet.Remove(currentNode);

            if(currentNode == end)
            {
                List<Node> path = new List<Node>();
                path.Insert(0, end);
            }
            path.Reverse();
            return path;
        }

        foreach(Node connectedNode in currentNode.connections)
        {
            float heldGScore = currentNode.gScore + Vector2.Distance(currentNode.transform.position, connectedNode.transform.position);
            if(heldGScore < connectedNode.gScore)
            {
                connectedNode.cameFrom = currentNode;
                connectedNode.gScore = heldGScore;
                connectedNode.hScore = Vector2.Distance(connectedNode.transform.position, end.transform.position);
                
                if(!openSet.Contains(connectedNode))
                {
                    openSet.Add(connectedNode);
                }
            }
        }
        return null;
    }
*/
}
