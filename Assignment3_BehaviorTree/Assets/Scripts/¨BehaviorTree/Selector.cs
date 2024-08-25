using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selector : Node
{

    public override NodeStatus Run(BlackBoard board)
    {
        //This time we need to count failures(in case of success we set status to success and return)
        int failureCount = 0;
        //Go through each child and find out if we already started him and if not, start it's behavior
        foreach(var child in children)
        {
            //If child didn't fail, it means, that it wasn't started
            if(child.status != NodeStatus.FAILURE)
            {
                NodeStatus childStatus = child.Run(board);
                //We can end here, because we need only 1 success to be successfull as selector (OR)
                if(childStatus == NodeStatus.SUCCESS)
                {
                    status = NodeStatus.SUCCESS;
                    return status;
                }
                else if(childStatus == NodeStatus.FAILURE)
                {
                    ++failureCount;
                }
                else
                {
                    break;
                }
            }
        }
        if(failureCount == children.Count)
        {
            status = NodeStatus.FAILURE;
            return status;
        }
        else
        {
            status = NodeStatus.RUNNING;
        }
        return status;
    }
}
