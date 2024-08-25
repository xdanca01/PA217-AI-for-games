using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aStar
{
    public class Node
    {
        public Vector2Int position;
        public float distanceFromStart;
        public Node parent;
        public Node(Vector2 position, float distance, Node p)
        {
            this.position = new Vector2Int(((int)position.x), ((int)position.y));
            this.distanceFromStart = distance;
            this.parent = p;
        }
    }

    List<Node> openNodes = new();
    List<Node> closedNodes = new();
    Maze parentMaze;
    string heuristic;
    Vector2 target;
    Node LastNode;

    public aStar(Vector2 start, Vector2 end, Maze m, string h)
    {
        List<Node> path = new();
        heuristic = h;
        target = end;
        parentMaze = m;
        Node node = new Node(start, 0.0f, null);
        LastNode = null;
        //ExpandNode(node);
        openNodes.Add(node);
    }

    Node FindNext()
    {
        Node best = openNodes[0];
        float bestDistanceHeuristic = DistanceByHeuristic(best);
        float nodeDistanceHeuristic;
        foreach(var node in openNodes)
        {
            nodeDistanceHeuristic = DistanceByHeuristic(node);
            if (node.distanceFromStart + nodeDistanceHeuristic < best.distanceFromStart + bestDistanceHeuristic)
            {
                best = node;
                bestDistanceHeuristic = nodeDistanceHeuristic;
            }
        }
        openNodes.Remove(best);
        ExpandNode(best);
        return best;
    }

    void ExpandNode(Node node)
    {
        CloseNode(node);
        //Go throught all tiles around me from [-1, -1] to [1, 1]
        for(int x = -1; x <= 1; ++x)
        {
            for (int y = -1; y <= 1; ++y)
            {
                Vector2Int newNodePosition = new Vector2Int(((int)node.position.x) + x, ((int)node.position.y) + y);
                //If the tile is free
                if(parentMaze.IsValidTileOfType(newNodePosition, MazeTileType.Free) == true)
                {
                    float newDistance = CalculateDistance(node.position, newNodePosition) + node.distanceFromStart;
                    Node next = new Node(newNodePosition, newDistance, node);
                    //The node is not already computed
                    if(IsClosed(next) == false && IsOpen(next) == false && CanMoveTo(next.parent, next))
                    {
                        parentMaze.SetFreeTileColor(newNodePosition, Color.green);
                        openNodes.Add(next);
                    }
                }
            }
        }
    }

    void CloseNode(Node node)
    {
        closedNodes.Add(node);
        Vector2Int pos = new Vector2Int(((int)node.position.x), ((int)node.position.y));
        parentMaze.SetFreeTileColor(pos, Color.red);
    }

    float CalculateDistance(Vector2 start, Vector2 target)
    {
        return (start - target).magnitude;
    }

    float DistanceByHeuristic(Node node)
    {
        if (heuristic == "Euclidean")
        {
            return (node.position - target).magnitude;
        }
        return 0.0f;
    }

    bool CanMoveTo(Node start, Node node)
    {
        int x = start.position.x - node.position.x;
        int y = start.position.y - node.position.y;
        if (x == 0 && y == 0) return true;
        Vector2Int tile1 = new Vector2Int(node.position.x + x, node.position.y);
        Vector2Int tile2 = new Vector2Int(node.position.x, node.position.y + y);
        if (parentMaze.IsValidTileOfType(tile1, MazeTileType.Free) && parentMaze.IsValidTileOfType(tile2, MazeTileType.Free)) return true;
        return false;
    }

    bool IsClosed(Node node)
    {
        foreach(var n in closedNodes)
        {
            if(n.position == node.position)
            {
                return true;
            }
        }
        return false;
    }

    bool IsOpen(Node node)
    {
        foreach (var n in openNodes)
        {
            if (n.position == node.position)
            {
                return true;
            }
        }
        return false;
    }

    public bool FindPath()
    {
        LastNode = FindNext();
        if (LastNode.position == target) return true;
        return false;
    }

    public List<Vector2Int> GetPath()
    {
        List<Vector2Int> path = new();
        Node LastNodeTmp = LastNode;
        while(LastNodeTmp != null)
        {
            path.Add(LastNodeTmp.position);
            LastNodeTmp = LastNodeTmp.parent;
        }
        path.Reverse();
        return path;
    }
}
 