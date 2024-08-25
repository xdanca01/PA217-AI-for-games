using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum NodeAction
{
    PLACEBOMB,
    MOVETOCLOSESTBREAKWALL,
    RUN,
    WAITFORBOMB,
    MOVETOAGENT,
    STAY
}
public class Action : Node
{
    public NodeAction action;
    private List<AgentAction> moveActions;
    private bool placedBomb = false;
    public override NodeStatus Run(BlackBoard board)
    {
        switch (action)
        {
            case NodeAction.PLACEBOMB:
                status = placeBomb(board);
                break;
            case NodeAction.MOVETOCLOSESTBREAKWALL:
                status = moveToClosestBreakWall(board);
                break;
            case NodeAction.RUN:
                status = run(board);
                break;
            case NodeAction.WAITFORBOMB:
                break;
            case NodeAction.MOVETOAGENT:
                break;
            case NodeAction.STAY:
                status = stay(board);
                break;
            default:
                status = NodeStatus.FAILURE;
                break;
        }
        return status;
    }

    public void SetAction(NodeAction a)
    {
        this.action = a;
    }

    private NodeStatus placeBomb(BlackBoard board)
    {
        if(placedBomb == false)
        {
            board.setAction(AgentAction.PlaceBomb);
            placedBomb = true;
            return NodeStatus.RUNNING;
        }
        return NodeStatus.SUCCESS;
    }

    private NodeStatus stay(BlackBoard board)
    {
        board.setAction(AgentAction.Stay);
        return NodeStatus.SUCCESS;
    }
    private NodeStatus run(BlackBoard board)
    {
        if(moveActions == null)
        {
            Vector2Int tileToHide = board.TileNotInSameRowCol();
            moveActions = board.brain.PathToBreakAbleWall(tileToHide, false).FindAll(b => b != AgentAction.Stay);
        }
        if (moveActions.Count >= 1)
        {
            //Move to the position on the path
            board.setAction(moveActions[0]);
            moveActions.RemoveAt(0);
        }
        //Already at position, so action is done
        if (moveActions.Count <= 0)
        {
            status = NodeStatus.SUCCESS;
            return status;
        }
        status = NodeStatus.RUNNING;
        return status;
    }


    private NodeStatus moveToClosestBreakWall(BlackBoard board)
    {
        //Find another agent and find path to it
        if(moveActions == null)
        {
            Agent anotherAgent = board.brain.AllAgents.Find(b => (b != board.agent));
            //Ignore stay actions(at wall)
            moveActions = board.brain.PathToBreakAbleWall(anotherAgent.CurrentTile).FindAll(b => b != AgentAction.Stay);
        }
        //Already at position, so action is done
        if(moveActions.Count <= 0)
        {
            status = NodeStatus.SUCCESS;
            return status;
        }
        //Move to the position on the path
        board.setAction(moveActions[0]);
        moveActions.RemoveAt(0);
        status = NodeStatus.RUNNING;
        return status;
    }
}
