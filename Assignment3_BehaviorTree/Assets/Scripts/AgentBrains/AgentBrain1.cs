using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentBrain1 : AgentBrain
{
    Node root;
    BlackBoard board;
    public override AgentAction GetNextAction()
    {
        if(board == null)
        {
            board = new BlackBoard(Agent, Maze, this);
        }
        //The tree has actions and conditions, only actions set actions, so after action is set, then return chosen action
        while(board.action == null)
        {
            //Create new tree, after one of subtrees was finished
            if (root == null || root.status == NodeStatus.FAILURE || root.status == NodeStatus.SUCCESS)
            {
                root = new Selector();
                //Tree 1 
                Sequence seq1 = new Sequence();
                Action moveToNextAgent = new Action();
                moveToNextAgent.SetAction(NodeAction.MOVETOCLOSESTBREAKWALL);
                Condition canPlaceBomb = new Condition();
                canPlaceBomb.SetCondition(Condition.NodeCondition.CanPlaceBomb);
                Action placeBomb = new Action();
                placeBomb.SetAction(NodeAction.PLACEBOMB);
                Action run = new Action();
                run.SetAction(NodeAction.RUN);
                //Tree 2
                Action stay = new Action();
                stay.SetAction(NodeAction.STAY);
                root.AddChild(seq1);
                root.AddChild(stay);
                seq1.AddChild(canPlaceBomb);
                seq1.AddChild(moveToNextAgent);
                seq1.AddChild(placeBomb);
                seq1.AddChild(run);
            }
            root.Run(board);
        }
        AgentAction tmp = (AgentAction)board.action;
        board.action = null;
        return tmp;
    }

    //Returns path to the destination tile, if it's not blocked by anything else(can be set to ignore DestructibleWalls)
    protected override AgentAction[] GetPathTo(Vector2Int destinationTile)
    {
        //Set PathFinding algorithm to get path to the destinationTile without ignoring DestructibleWalls (If this is not wanted solution, then set last argument to true)
        PathFinding algo = new PathFinding(Agent.CurrentTile, destinationTile, Maze, false);
        List<Vector2Int> path = algo.GetPath();
        AgentAction[] actions = convertPathToActions(path, false);
        return actions;
    }

    //does almost the same as GetPathTo, but I can say, if I want to use or not use breakWalls, because I want both cases
    public List<AgentAction> PathToBreakAbleWall(Vector2Int destinationTile, bool breakWalls = true)
    {
        PathFinding algo = new PathFinding(Agent.CurrentTile, destinationTile, Maze, breakWalls);
        List<Vector2Int> path = algo.GetPath();
        AgentAction[] actions = convertPathToActions(path, true);
        return new List<AgentAction>(actions);
    }

    //Method to know, if I can go to the free tile, because I need it for the escape away from bomb
    public bool PathToTileExists(Vector2Int destinationTile)
    {
        PathFinding algo = new PathFinding(Agent.CurrentTile, destinationTile, Maze, false);
        List<Vector2Int> path = algo.GetPath();
        if(path.Count > 0)
        {
            return true;
        }
        return false;
    }

    //Converts the output from algorithm to the AgentActions
    private AgentAction[] convertPathToActions(List<Vector2Int> path, bool stopAtWall)
    {
        AgentAction[] actions = new AgentAction[path.Count];
        Vector2Int previous = Agent.CurrentTile;
        Vector2Int difference;
        for (int i = 0; i < path.Count; ++i)
        {
            //If I don't want to include breakable walls to the path, then set stopAtWall is set to true, so the actions stop at first breakable wall
            if (stopAtWall == true && MazeTileType.Free != Maze.GetTileType(path[i]))
            {
                break;
            }
            difference = path[i] - previous;
            previous = path[i];
            AgentAction action = AgentAction.Stay;
            if (difference.y < 0)
            {
                action = AgentAction.MoveUp;
            }
            else if (difference.y > 0)
            {
                action = AgentAction.MoveDown;
            }
            else if (difference.x > 0)
            {
                action = AgentAction.MoveRight;
            }
            else if(difference.x < 0)
            {
                action = AgentAction.MoveLeft;
            }
            actions[i] = action;
        }
        return actions;
    }
}
