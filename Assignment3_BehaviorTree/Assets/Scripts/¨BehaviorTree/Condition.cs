using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Condition : Node
{
    public enum NodeCondition
    {
        CanMove,
        CanPlaceBomb,
    }

    private NodeCondition condition;

    public override NodeStatus Run(BlackBoard board)
    {
        switch (condition)
        {
            case NodeCondition.CanPlaceBomb:
                status = canPlaceBomb(board);
                break;
            default:
                break;
        }
        return status;
    }

    public void SetCondition(NodeCondition c)
    {
        condition = c;
    }

    //I have enough bombs to place
    private NodeStatus canPlaceBomb(BlackBoard board)
    {
        //I have bomb
        if(1 <= board.agent.BombsPlacedCount)
        {
            status = NodeStatus.FAILURE;
            return status;
        }
        //I don't stand on bomb
        Bomb bomb = board.brain.ActiveBombs.Find(b => b.TileLocation == board.agent.CurrentTile);
        if(bomb != null)
        {
            status = NodeStatus.FAILURE;
            return status;
        }
        return NodeStatus.SUCCESS;
    }

    private bool destructibleWallExists(Maze test)
    {
        foreach(var row in test.GetMazeTiles())
        {
            foreach (var tile in row)
            {
                if (tile == MazeTileType.DestructibleWall)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private bool canMove(Maze maze, Vector2Int nextTile)
    {
        //TODO is there bomb?
        if(maze.GetTileType(nextTile) == MazeTileType.Free)
        {
            return true;
        }
        return false;
    }

    
}
