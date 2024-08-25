using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sequence : Node
{

    public override NodeStatus Run(BlackBoard board)
    {
        //Count children that succeded, if they all succeded we can set our status to SUCCESS, otherwise it's running or in case of failure fail
        int successCount = 0;
        //Go through all children
        foreach(var child in children)
        {
            //We didn't run the child yet
            if(child.status != NodeStatus.SUCCESS)
            {
                //Run child
                NodeStatus childStatus = child.Run(board);
                //If the child fails the whole sequence fails (AND)
                if(childStatus == NodeStatus.FAILURE)
                {
                    status = NodeStatus.FAILURE;
                    return status;
                }
                else if(childStatus == NodeStatus.SUCCESS)
                {
                    ++successCount;
                }
                else
                {
                    break;
                }
            }
            //Count the number of successfully ended childs, to know if we can end the sequence with success
            else
            {
                ++successCount;
            }
        }
        if(successCount == children.Count)
        {
            status = NodeStatus.SUCCESS;
        }
        else
        {
            status = NodeStatus.RUNNING;
        }
        return status;
    }
}
