using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinding : MonoBehaviour
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
    Vector2 target;
    Node LastNode;
    bool breakWalls;

    public PathFinding(Vector2 start, Vector2 end, Maze m, bool canIbreakWalls)
    {
        target = end;
        parentMaze = m;
        Node node = new Node(start, 0.0f, null);
        LastNode = null;
        //ExpandNode(node);
        openNodes.Add(node);
        breakWalls = canIbreakWalls;
    }

    Node FindNext()
    {
        if(openNodes.Count <= 0)
        {
            return null;
        }
        Node best = openNodes[0];
        float bestDistanceHeuristic = DistanceByHeuristic(best);
        float nodeDistanceHeuristic;
        foreach (var node in openNodes)
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
        for (int x = -1; x <= 1; ++x)
        {
            for (int y = -1; y <= 1; ++y)
            {
                //Get the tile based on x,y
                Vector2Int newNodePosition = new Vector2Int(((int)node.position.x) + x, ((int)node.position.y) + y);
                //If the tile is free or destructible
                if (isTileValid(newNodePosition))
                {
                    //Compute the cost by heuristics + previous node
                    float newDistance = CalculateDistance(node.position, newNodePosition) + node.distanceFromStart;
                    Node next = new Node(newNodePosition, newDistance, node);
                    //The node is not already computed (is not closed, already open or it is valid move)
                    if (IsClosed(next) == false && IsOpen(next) == false && CanMoveTo(next.parent, next))
                    {
                        openNodes.Add(next);
                    }
                }
            }
        }
    }

    private bool isTileValid(Vector2Int nodePosition)
    {
        //If I can break walls, then the valid type of tile is FREE and DESTRUCTIBLE WALLS
        if(breakWalls == true)
        {
            if(parentMaze.IsValidTileOfType(nodePosition, MazeTileType.Free) || parentMaze.IsValidTileOfType(nodePosition, MazeTileType.DestructibleWall))
            {
                return true;
            }
        }
        //Can move only to free tiles
        else
        {
            if (parentMaze.IsValidTileOfType(nodePosition, MazeTileType.Free))
            {
                return true;
            }
        }
        return false;
    }

    void CloseNode(Node node)
    {
        closedNodes.Add(node);
        Vector2Int pos = new Vector2Int(((int)node.position.x), ((int)node.position.y));
    }

    float CalculateDistance(Vector2 start, Vector2 target)
    {
        return (start - target).magnitude;
    }

    //Only euclidean
    float DistanceByHeuristic(Node node)
    {
        return (node.position - target).magnitude;
    }

    bool CanMoveTo(Node start, Node node)
    {
        int x = start.position.x - node.position.x;
        int y = start.position.y - node.position.y;
        //Can move only in one direction
        if (Mathf.Abs(x) > 0 && Mathf.Abs(y) > 0) return false;
        //Check type of tile (Free or destructible)
        if (Mathf.Abs(x) > 0)
        {
            Vector2Int tile1 = new Vector2Int(node.position.x + x, node.position.y);
            if (isTileValid(tile1))
            {
                return true;
            }
        }
        //Check type of tile (Free or destructible)
        if (Mathf.Abs(y) > 0)
        {
            Vector2Int tile2 = new Vector2Int(node.position.x, node.position.y + y);
            if (isTileValid(tile2))
            {
                return true;
            }
        }
        return false;
    }

    bool IsClosed(Node node)
    {
        foreach (var n in closedNodes)
        {
            if (n.position == node.position)
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

    //Performs step in path finding (Finds the next best option from open nodes)
    public bool FindPath()
    {
        LastNode = FindNext();
        if (LastNode == null || LastNode.position == target) return true;
        return false;
    }

    public List<Vector2Int> GetPath()
    {
        while (!FindPath()) ;
        List<Vector2Int> path = new();
        Node LastNodeTmp = LastNode;
        while (LastNodeTmp != null)
        {
            path.Add(LastNodeTmp.position);
            LastNodeTmp = LastNodeTmp.parent;
        }
        if(path.Count > 0)
        {
            path.RemoveAt(path.Count - 1);
            path.Reverse();
        }
        return path;
    }
}
